using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Exceptions;
using NEventStore.Logging;
using NEventStore.Persistence.EventStore.Events;
using NEventStore.Persistence.EventStore.Extensions;
using NEventStore.Persistence.EventStore.Models;
using NEventStore.Persistence.EventStore.Services;

namespace NEventStore.Persistence.EventStore
{
    public class EventStorePersistenceEngine : IPersistStreams
    {
        private readonly List<string> _buckets;
        private readonly IEventStoreConnection _connection;
        private readonly IEventStoreSerializer _serializer;
        private readonly IStreamNamingStrategy _namingStrategy;
        private readonly EventStorePersistenceOptions _options;
        
        private class VersionRange
        {

            public VersionRange(int minVersion, int maxVersion)
            {
                MinVersion = TranslateVersion(minVersion);
                MaxVersion = TranslateVersion(maxVersion);
                EventCount = MaxVersion - MinVersion + 1;
                
            }
            private int TranslateVersion(int streamVersion)
            {
                if (streamVersion > 0)
                {
                    return streamVersion - 1;
                }
                return 0;
            }
            public int MinVersion { get; private set; }
            public int MaxVersion { get; private set; }
            public int EventCount { get; private set; }
        }
        private bool _disposed;
        
        public EventStorePersistenceEngine(IEventStoreConnection connection, IEventStoreSerializer serializer,IStreamNamingStrategy namingStrategy,EventStorePersistenceOptions options)
        {
            _connection = connection;
            _serializer = serializer;
            _namingStrategy = namingStrategy;
            _options = options;
            
            _buckets = new List<string>();
        }

        public void Dispose()
        {
            _disposed = true;
        }

        public IEnumerable<ICommit> GetFrom(string bucketId, string streamId, int minRevision, int maxRevision)
        {
            var range = new VersionRange(minRevision, maxRevision);
            StreamEventsSlice slice = _connection.ReadStreamEventsForwardAsync(_namingStrategy.CreateStream(bucketId, streamId),
                range.MinVersion, range.EventCount, true).Result;
            PersistentEvent[] events = slice.Events.Select(evt =>
                new PersistentEvent(evt, _serializer)).ToArray();
            return
                events.GroupBy(c => c.CommitId)
                    .Select(
                        g =>
                        {
                            PersistentEvent first = g.First();
                            return new Commit(bucketId, streamId, first.StreamRevision, g.Key, first.CommitSequence,
                                first.CommitStamp, string.Empty, first.GetCommitHeaders(),
                                g.Select(e => e.ToEventMessage()))
                                ;
                        });
        }

        public ICommit Commit(CommitAttempt attempt)
        {
            string streamId = attempt.GetStreamName(_namingStrategy);
            EventStoreTransaction transaction=null;
            try
            {
                
                if (!_buckets.Contains(attempt.BucketId))
                {
                    AddBucket(attempt.BucketId);
                }
                if (attempt.ExpectedVersion() == ExpectedVersion.NoStream)
                {
                    AddStream(attempt.BucketId, attempt.StreamId);
                }
                
                var eventsToSave =
                       attempt.Events.Select(evt => new PersistentEvent(evt, attempt).ToEventData(_serializer)).ToArray();

                //The reason to write the events directly and not the commits is to maintain the event type intact in the event store
                //This can facilitate the writing of projections and listeners do not need to know anything about neventstore
                transaction = _connection.StartTransactionAsync(streamId, attempt.ExpectedVersion()).Result;
                
                var position = 0;
                while (position < eventsToSave.Length)
                {
                    var pageEvents = eventsToSave.Skip(position).Take(_options.WritePageSize);
                    transaction.WriteAsync(pageEvents).Wait();
                    position += _options.WritePageSize;
                }
                ///TODO:Solve this issue
                //The Commit stream is only being used to ensure that no commit is duplicated, but since there`s no interstream transaction support in GES this can be risk.
                //If for some reason the commit is writted and the events are not there`ll be a lock and no futher event will be written
                WriteCommit(attempt);
                CheckSnapshotThreshold(attempt);
                var result = transaction.CommitAsync().Result;
             
                return attempt.ToCommit(result);
            }
            catch (AggregateException ex)
            {
                if (transaction != null)
                {
                    try
                    {
                        
                        transaction.Rollback();
                    }
                    catch (Exception)
                    {
                        //if the error happens inside the Commit, the transaction is aborted!??
                    }
                    
                }
                foreach (var exception in ex.InnerExceptions   )
                {
                    if (exception is WrongExpectedVersionException)
                    {
                        throw new ConcurrencyException(exception.Message,exception);
                    }
                }
                LogFactory.BuildLogger(GetType()).Error(ex.ToString());
                throw;
            }
            
        }

        private void CheckSnapshotThreshold(CommitAttempt attempt)
        {
            if (attempt.StreamRevision > _options.MinimunSnapshotThreshold)
            {
                var isSnapShotCandidate = false;
                var metadata = _connection.GetStreamMetadataAsync(attempt.GetStreamName(_namingStrategy))
                    .Result.StreamMetadata;
                metadata.TryGetValue(MetadataKeys.IsSnapShotCandidate, out isSnapShotCandidate);
                if (!isSnapShotCandidate)
                {
                    var newData = metadata.Clone().SetCustomProperty(MetadataKeys.IsSnapShotCandidate, true).Build();
                    _connection.SetStreamMetadataAsync(attempt.GetStreamName(_namingStrategy), ExpectedVersion.Any,
                        newData).Wait();
                    _connection.AppendToStreamAsync(_namingStrategy.CreateStreamsToSnapshot(attempt.BucketId),ExpectedVersion.Any,
                        new SnapshotThresholdReached() {StreamId = attempt.StreamId}.ToEventData(_serializer)).Wait();
                }

            }
        }

        private void WriteCommit(CommitAttempt attempt)
        {
            _connection.AppendToStreamAsync(attempt.CreateStreamCommitsName(_namingStrategy), attempt.ExpectedCommitVersion(),
                attempt.ToEventData(_serializer)).Wait();
        }

        public ISnapshot GetSnapshot(string bucketId, string streamId, int maxRevision)
        {
            StreamEventsSlice currentSlice;
            var nextSliceStart = StreamPosition.End;
            do
            {
                currentSlice =
                _connection.ReadStreamEventsBackwardAsync(_namingStrategy.CreateStreamSnapshots(bucketId,streamId), nextSliceStart,
                                                              _options.ReadPageSize,true)
                                                              .Result;
                foreach (var resolvedEvent in currentSlice.Events)
                {
                    var snapShot = resolvedEvent.Event.ToSnapshot(_serializer);
                    if (snapShot.StreamRevision <= maxRevision)
                        return snapShot;
                }

                
                nextSliceStart = currentSlice.NextEventNumber;
            } while (!currentSlice.IsEndOfStream);
            return null;

        }

        public bool AddSnapshot(ISnapshot snapshot)
        {
            _connection.AppendToStreamAsync(snapshot.GetStreamName(_namingStrategy), ExpectedVersion.Any,
                snapshot.ToEventData(_serializer)).Wait();
            return true;
            
            

        }
        /// <summary>
        /// This is a very slow operation, use with caution
        /// </summary>
        /// <param name="bucketId"></param>
        /// <param name="maxThreshold"></param>
        /// <returns></returns>
        public IEnumerable<IStreamHead> GetStreamsToSnapshot(string bucketId, int maxThreshold)
        {
            
            var retval = new List<IStreamHead>();
            StreamEventsSlice currentSlice;
            var nextSliceStart = StreamPosition.Start;
            do
            {
                currentSlice =
                    _connection.ReadStreamEventsForwardAsync(_namingStrategy.CreateStreamsToSnapshot(bucketId),
                        nextSliceStart, _options.ReadPageSize, true).Result;
                foreach (var resolvedEvent in currentSlice.Events)
                {
                    var evt = _serializer.Deserialize<SnapshotThresholdReached>(resolvedEvent.Event.Data);
                    var headRevision =
                        _connection.ReadStreamEventsBackwardAsync(_namingStrategy.CreateStream(bucketId, evt.StreamId),
                            StreamPosition.End, 1, true).Result.LastEventNumber + 1;
                    var lastSnapShots =
                        _connection.ReadStreamEventsBackwardAsync(
                            _namingStrategy.CreateStreamSnapshots(bucketId, evt.StreamId), StreamPosition.End, 1, true).Result;
                    var snapshotRevision = 0;
                    if (lastSnapShots.LastEventNumber >= 0)
                    {
                        var snapShot = lastSnapShots.Events.FirstOrDefault().Event.ToSnapshot(_serializer);
                        snapshotRevision = snapShot.StreamRevision;
                    }
                    if (headRevision - snapshotRevision  > _options.MinimunSnapshotThreshold)
                    {
                        retval.Add(new StreamHead(bucketId, evt.StreamId, headRevision, snapshotRevision));    
                    }
                    

                }
                nextSliceStart = currentSlice.NextEventNumber;
            } while (!currentSlice.IsEndOfStream);
            return retval;
        }

        public void Initialize()
        {
            ///TODO:Start listening for changes in the bucket stream after the initial load
            _connection.ActOnAll<BucketCreated>(_namingStrategy.BucketsStream, evt => _buckets.Add(evt.Bucket), _serializer);
        }

        public IEnumerable<ICommit> GetFrom(string bucketId, DateTime start)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ICommit> GetFrom(string checkpointToken = null)
        {
            throw new NotImplementedException();
        }

        public ICheckpoint GetCheckpoint(string checkpointToken = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ICommit> GetFromTo(string bucketId, DateTime start, DateTime end)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ICommit> GetUndispatchedCommits()
        {
            throw new NotImplementedException();
        }

        public void MarkCommitAsDispatched(ICommit commit)
        {
            throw new NotImplementedException();
        }

        public void Purge()
        {
            foreach (string bucket in _buckets)
            {
                Purge(bucket);
            }
            _connection.DeleteStreamAsync(_namingStrategy.BucketsStream, ExpectedVersion.Any).Wait();
        }

        public void Purge(string bucketId)
        {
            string streamId = _namingStrategy.CreateBucketStreamsStream(bucketId);
            _connection.ActOnAll<StreamCreated>(streamId,
                evt => DeleteStream(evt.BucketId, evt.StreamId), _serializer);
            _connection.DeleteStreamAsync(streamId, ExpectedVersion.Any).Wait();
            _connection.DeleteStreamAsync(_namingStrategy.CreateStreamsToSnapshot(bucketId), ExpectedVersion.Any).Wait();
        }

        public void Drop()
        {
            throw new NotImplementedException();
        }

        public void DeleteStream(string bucketId, string streamId)
        {
            _connection.DeleteStreamAsync(_namingStrategy.CreateStream(bucketId, streamId), ExpectedVersion.Any).Wait();
            _connection.DeleteStreamAsync(_namingStrategy.CreateStreamCommits(bucketId, streamId), ExpectedVersion.Any).Wait();
            _connection.DeleteStreamAsync(_namingStrategy.CreateStreamSnapshots(bucketId, streamId), ExpectedVersion.Any).Wait();
        }

        public bool IsDisposed
        {
            get { return _disposed; }
        }

        

        private void AddStream(string bucketId, string streamId)
        {
            _connection.AppendToStreamAsync(_namingStrategy.CreateBucketStreamsStream(bucketId), ExpectedVersion.Any,
                new StreamCreated {BucketId = bucketId, StreamId = streamId}.ToEventData(_serializer)).Wait();
        }

        private void AddBucket(string bucketId)
        {
            _buckets.Add(bucketId);
            _connection.AppendToStreamAsync(_namingStrategy.BucketsStream, ExpectedVersion.Any,
                new BucketCreated {Bucket = bucketId}.ToEventData(_serializer)).Wait();

        }

       
    }
}