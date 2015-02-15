using System;
using System.Collections.Generic;
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
            StreamEventsSlice slice = _connection.ReadStreamEventsForwardAsync(_namingStrategy.CreateStreamName(bucketId, streamId),
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

                transaction = _connection.StartTransactionAsync(streamId, attempt.ExpectedVersion()).Result;

                var position = 0;
                while (position < eventsToSave.Length)
                {
                    var pageEvents = eventsToSave.Skip(position).Take(_options.WritePageSize);
                    transaction.WriteAsync(pageEvents).Wait();
                    position += _options.WritePageSize;
                }
                WriteCommit(attempt);
                return attempt.ToCommit(transaction.CommitAsync().Result);
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
                _connection.ReadStreamEventsBackwardAsync(_namingStrategy.CreateStreamSnapshotsName(bucketId,streamId), nextSliceStart,
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

        public IEnumerable<IStreamHead> GetStreamsToSnapshot(string bucketId, int maxThreshold)
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            ///TODO:Start listening for changes in the bucket stream after the initial load
            _connection.ActOnAll<BucketCreated>(_namingStrategy.BucketsStreamName, evt => _buckets.Add(evt.Bucket), _serializer);
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
            _connection.DeleteStreamAsync(_namingStrategy.BucketsStreamName, ExpectedVersion.Any).Wait();
        }

        public void Purge(string bucketId)
        {
            string streamId = _namingStrategy.CreateBucketStreamsStreamName(bucketId);
            _connection.ActOnAll<StreamCreated>(streamId,
                evt => DeleteStream(evt.BucketId, evt.StreamId), _serializer);
            _connection.DeleteStreamAsync(streamId, ExpectedVersion.Any).Wait();
        }

        public void Drop()
        {
            throw new NotImplementedException();
        }

        public void DeleteStream(string bucketId, string streamId)
        {
            _connection.DeleteStreamAsync(_namingStrategy.CreateStreamName(bucketId, streamId), ExpectedVersion.Any).Wait();
        }

        public bool IsDisposed
        {
            get { return _disposed; }
        }

        

        private void AddStream(string bucketId, string streamId)
        {
            _connection.AppendToStreamAsync(_namingStrategy.CreateBucketStreamsStreamName(bucketId), ExpectedVersion.Any,
                new StreamCreated {BucketId = bucketId, StreamId = streamId}.ToEventData(_serializer)).Wait();
        }

        private void AddBucket(string bucketId)
        {
            _buckets.Add(bucketId);
            _connection.AppendToStreamAsync(_namingStrategy.BucketsStreamName, ExpectedVersion.Any,
                new BucketCreated {Bucket = bucketId}.ToEventData(_serializer)).Wait();

        }

       
    }
}