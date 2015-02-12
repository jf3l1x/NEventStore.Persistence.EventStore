using System;
using EventStore.ClientAPI;

namespace NEventStore.Persistence.GES.Extensions
{
    public static class CommitAttemptExtensions
    {
        public static string GetHashedStreamName(this CommitAttempt attempt)
        {
            return string.Format("NES.{0}.{1}", attempt.BucketId, attempt.StreamId).ToHashRepresentation();
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
                result.NextExpectedVersion - 1,
                attempt.CommitId,
                (int)result.LogPosition.CommitPosition,
                DateTime.UtcNow,
                result.LogPosition.CommitPosition.ToString(),
                attempt.Headers, attempt.Events);

        }
    }
}