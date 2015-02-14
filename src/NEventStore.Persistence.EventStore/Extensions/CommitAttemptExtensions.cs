using System;
using EventStore.ClientAPI;
using NEventStore.Persistence.GES.Services;

namespace NEventStore.Persistence.GES.Extensions
{
    public static class CommitAttemptExtensions
    {
        public static string GetHashedStreamName(this CommitAttempt attempt)
        {
            return string.Format("NES.{0}.{1}", attempt.BucketId, attempt.StreamId).ToHashRepresentation();
        }

        public static string GetHashedCommitStreamName(this CommitAttempt attempt)
        {
            return string.Format("NES.{0}.{1}.COMMITS", attempt.BucketId, attempt.StreamId).ToHashRepresentation();
        }
        public static int ExpectedCommitVersion(this CommitAttempt attempt)
        {
            var expected = attempt.CommitSequence - 2;
            if (expected == -1)
            {
                return EventStore.ClientAPI.ExpectedVersion.NoStream;
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
                return EventStore.ClientAPI.ExpectedVersion.NoStream;
            }
            return expected;
        }
    }
}