using System;
using System.Collections.Generic;
using EventStore.ClientAPI;
using NEventStore.Persistence.EventStore.Events;
using NEventStore.Persistence.EventStore.Extensions;
using NEventStore.Persistence.EventStore.Services.Naming;

namespace NEventStore.Persistence.EventStore.Services.Control
{
    internal class NoProjectionStrategy : IControlStrategy
    {
        private readonly List<string> _buckets;
        private readonly IEventStoreConnection _connection;
        private readonly IStreamNamingStrategy _namingStrategy;
        private readonly EventStorePersistenceOptions _options;
        private readonly IEventStoreSerializer _serializer;

        public NoProjectionStrategy(IEventStoreConnection connection, EventStorePersistenceOptions options,
            IStreamNamingStrategy namingStrategy, IEventStoreSerializer serializer)
        {
            _connection = connection;
            _options = options;
            _namingStrategy = namingStrategy;
            _serializer = serializer;
            _buckets = new List<string>();
        }

        public void Initialize()
        {

            ///TODO:Start listening for changes in the bucket stream after the initial load
            _connection.ActOnAll<string>(_namingStrategy.BucketsStream, evt => _buckets.Add(evt), _serializer, _options.UserCredentials);
        }

        public void PreProcessCommitAttempt(CommitAttempt attempt)
        {
            if (!_buckets.Contains(attempt.BucketId))
            {
                AddBucket(attempt.BucketId);
            }
            if (attempt.ExpectedVersion() == ExpectedVersion.NoStream)
            {
                AddStream(attempt.BucketId, attempt.StreamId);
            }
        }

        public void PostProcessCommitAttempt(CommitAttempt attempt)
        {
            ///TODO:Solve this issue
            //The Commit stream is only being used to ensure that no commit is duplicated, but since there`s no interstream transaction support in GES this can be risk.
            //If for some reason the commit is writted and the events are not there`ll be a lock and no futher event will be written
            WriteCommit(attempt);
            CheckSnapshotThreshold(attempt);
        }

        private void AddStream(string bucketId, string streamId)
        {
            _connection.AppendToStreamAsync(_namingStrategy.CreateBucketStreamsStream(bucketId), ExpectedVersion.Any,_options.UserCredentials,
                new StreamCreated {BucketId = bucketId, StreamId = streamId}.ToEventData(_serializer)).Wait();
        }

        private void AddBucket(string bucketId)
        {
            _buckets.Add(bucketId);
            _connection.AppendToStreamAsync(_namingStrategy.BucketsStream, ExpectedVersion.Any, _options.UserCredentials,
                 bucketId.ToEventData(_serializer)).Wait();
        }

        private void CheckSnapshotThreshold(CommitAttempt attempt)
        {
            
            if (attempt.StreamRevision % _options.MinimunSnapshotThreshold<attempt.Events.Count)
            {
                //there's an event inside the commit that is multiple of _options.MinimunSnapshotThreshold
                _connection.AppendToStreamAsync(_namingStrategy.CreateStreamsToSnapshot(attempt.BucketId),
               ExpectedVersion.Any, _options.UserCredentials,
               attempt.CreateRevisionUpdate().ToEventData(_serializer)).Wait();
            }
        }

        private void WriteCommit(CommitAttempt attempt)
        {

            _connection.AppendToStreamAsync(attempt.CreateStreamCommitsName(_namingStrategy),
                attempt.ExpectedCommitVersion(), _options.UserCredentials,
                attempt.ToEventData(_serializer)).Wait();
        }
    }
}