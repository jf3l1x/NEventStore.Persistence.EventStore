using System;
using System.Collections.Generic;
using System.Linq;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Exceptions;
using NEventStore.Logging;
using NEventStore.Persistence.GES.Events;
using NEventStore.Persistence.GES.Extensions;
using NEventStore.Persistence.GES.Models;
using NEventStore.Persistence.GES.Services;

namespace NEventStore.Persistence.GES
{
    public class GESPersistenceEngine : IPersistStreams
    {
        private const string NES_BUCKETS = "NES.BUCKETS";
        private readonly List<string> _buckets;
        private readonly IEventStoreConnection _connection;
        private readonly IEventStoreSerializer _serializer;
        private const int WritePageSize = 500;
        private const int ReadPageSize = 500;

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
        
        public GESPersistenceEngine(IEventStoreConnection connection, IEventStoreSerializer serializer)
        {
            _connection = connection;
            _serializer = serializer;

            _buckets = new List<string>();
        }

        public void Dispose()
        {
            _disposed = true;
        }

        public IEnumerable<ICommit> GetFrom(string bucketId, string streamId, int minRevision, int maxRevision)
        {
            var range = new VersionRange(minRevision, maxRevision);
            StreamEventsSlice slice = _connection.ReadStreamEventsForwardAsync(HashStreamName(bucketId, streamId),
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
            string streamId = attempt.GetHashedStreamName();
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
                    var pageEvents = eventsToSave.Skip(position).Take(WritePageSize);
                    transaction.WriteAsync(pageEvents).Wait();
                    position += WritePageSize;
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
            _connection.AppendToStreamAsync(attempt.GetHashedCommitStreamName(), attempt.ExpectedCommitVersion(),
                attempt.ToEventData(_serializer)).Wait();
        }

        public ISnapshot GetSnapshot(string bucketId, string streamId, int maxRevision)
        {
            throw new NotImplementedException();
        }

        public bool AddSnapshot(ISnapshot snapshot)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IStreamHead> GetStreamsToSnapshot(string bucketId, int maxThreshold)
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            ///TODO:Start listening for changes in the bucket stream after the initial load
            _connection.ActOnAll<BucketCreated>(NES_BUCKETS, evt => _buckets.Add(evt.Bucket), _serializer);
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
            _connection.DeleteStreamAsync(NES_BUCKETS, ExpectedVersion.Any).Wait();
        }

        public void Purge(string bucketId)
        {
            string streamId = CreateBucketStreamsStreamName(bucketId);
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
            _connection.DeleteStreamAsync(HashStreamName(bucketId, streamId), ExpectedVersion.Any).Wait();
        }

        public bool IsDisposed
        {
            get { return _disposed; }
        }

        

        private void AddStream(string bucketId, string streamId)
        {
            _connection.AppendToStreamAsync(CreateBucketStreamsStreamName(bucketId), ExpectedVersion.Any,
                new StreamCreated {BucketId = bucketId, StreamId = streamId}.ToEventData(_serializer)).Wait();
        }

        private void AddBucket(string bucketId)
        {
            _buckets.Add(bucketId);
            _connection.AppendToStreamAsync(NES_BUCKETS, ExpectedVersion.Any,
                new BucketCreated {Bucket = bucketId}.ToEventData(_serializer)).Wait();

        }

        private string CreateBucketStreamsStreamName(string bucketId)
        {
            return string.Format("NES.{0}.STREAMS", bucketId).ToHashRepresentation();
        }

        private string HashStreamName(string bucketId, string streamId)
        {
            return string.Format("NES.{0}.{1}", bucketId, streamId).ToHashRepresentation();
        }
    }
}