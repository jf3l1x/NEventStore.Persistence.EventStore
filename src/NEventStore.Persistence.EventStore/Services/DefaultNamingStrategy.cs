namespace NEventStore.Persistence.EventStore.Services
{
    public class DefaultNamingStrategy : IStreamNamingStrategy
    {
        public string BucketsStreamName
        {
            get { return "NES.BUCKETS"; }
        }

        public string CreateBucketStreamsStreamName(string bucketId)
        {
            return string.Format("NES.{0}.STREAMS", bucketId);
        }

        public string CreateStreamName(string bucketId, string streamId)
        {
            return string.Format("NES.{0}.{1}", bucketId, streamId);
        }

        public string CreateStreamCommitsName(string bucketId, string streamId)
        {
            return string.Format("NES.{0}.{1}.COMMITS", bucketId, streamId);
        }

        public string CreateStreamSnapshotsName(string bucketId, string streamId)
        {
            return string.Format("NES.{0}.{1}.SNAPSHOTS", bucketId, streamId);
        }
    }
}