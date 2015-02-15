using System;
using EventStore.ClientAPI;
using NEventStore.Persistence.EventStore.Services;

namespace NEventStore.Persistence.EventStore.Extensions
{
    public static class CommitAttemptExtensions
    {
        public static string GetStreamName(this CommitAttempt attempt,IStreamNamingStrategy namingStrategy)
        {
            return namingStrategy.CreateStreamName(attempt.BucketId, attempt.StreamId);
        }
        public static string CreateStreamCommitsName(this CommitAttempt attempt, IStreamNamingStrategy namingStrategy)
        {
            return namingStrategy.CreateStreamCommitsName(attempt.BucketId, attempt.StreamId);
        }
       
        public static int ExpectedCommitVersion(this CommitAttempt attempt)
        {
            var expected = attempt.CommitSequence - 2;
            if (expected == -1)
            {
                return global::EventStore.ClientAPI.ExpectedVersion.NoStream;
            }
            return expected;
        }

        public static EventData ToEventData(this CommitAttempt attempt,IEventStoreSerializer serializer)
        {
            return new EventData(attempt.CommitId, typeof(CommitAttempt).FullName, serializer.IsJsonSerializer, serializer.Serialize(attempt),new byte[0]);
        }
        public static ICommit ToCommit(this CommitAttempt attempt,WriteResult result)
        {
            if (attempt == null)
            {
                return null;
            }
            return new Commit(
                attempt.BucketId,
                attempt.StreamId,
                result.NextExpectedVersion,
                attempt.CommitId,
                attempt.CommitSequence,
                DateTime.UtcNow,
                result.LogPosition.CommitPosition.ToString(),
                attempt.Headers, attempt.Events);

        }

        public static int ExpectedVersion(this CommitAttempt attempt)
        {
            var expected= attempt.StreamRevision - attempt.Events.Count-1;
            if (expected == -1)
            {
                return global::EventStore.ClientAPI.ExpectedVersion.NoStream;
            }
            return expected;
        }
    }
}