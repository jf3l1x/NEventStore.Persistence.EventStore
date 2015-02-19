namespace NEventStore.Persistence.EventStore.Services
{
    public interface IStreamNamingStrategy
    {
        string BucketsStream { get; }
        string CreateBucketStreamsStream(string bucketId);
        string CreateStream(string bucketId, string streamId);
        string CreateStreamCommits(string bucketId, string streamId);
        string CreateStreamSnapshots(string bucketId, string streamId);
        string CreateStreamsToSnapshot(string bucketId);
    }
}