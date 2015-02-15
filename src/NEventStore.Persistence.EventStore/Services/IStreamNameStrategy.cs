namespace NEventStore.Persistence.EventStore.Services
{
    public interface IStreamNamingStrategy
    {
        string BucketsStreamName { get; }
        string CreateBucketStreamsStreamName(string bucketId);
        string CreateStreamName(string bucketId, string streamId);
        string CreateStreamCommitsName(string bucketId, string streamId);
        string CreateStreamSnapshotsName(string bucketId, string streamId);
    }
}