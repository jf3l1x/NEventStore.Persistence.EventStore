namespace NEventStore.Persistence.EventStore.Services.Naming
{
    public class DefaultNamingStrategy : IStreamNamingStrategy
    {
        public string BucketsStream
        {
            get { return "nesBuckets"; }
        }

        public string CreateBucketStreamsStream(string bucketId)
        {
            return string.Format("nesStreams-{0}", bucketId);
        }

        public string CreateStream(string bucketId, string streamId)
        {
            return string.Format("nesEvents-{0}.{1}", bucketId, streamId);
        }

        public string CreateStreamCommits(string bucketId, string streamId)
        {
            return string.Format("nesCommits-{0}.{1}", bucketId, streamId);
        }

        public string CreateStreamSnapshots(string bucketId, string streamId)
        {
            return string.Format("nesSnapshots-{0}.{1}", bucketId, streamId);
        }

        public string CreateStreamsToSnapshot(string bucketId)
        {
            return string.Format("nesStreamsToSnapshot-{0}", bucketId);
        }
    }
}