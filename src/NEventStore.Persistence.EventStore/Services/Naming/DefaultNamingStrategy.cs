namespace NEventStore.Persistence.EventStore.Services.Naming
{
    public class DefaultNamingStrategy : IStreamNamingStrategy
    {
        public string BucketsStream
        {
            get { return "nesbuckets"; }
        }

        public string CreateBucketStreamsStream(string bucketId)
        {
            return string.Format("nesstreams-{0}", bucketId);
        }

        public string CreateStream(string bucketId, string streamId)
        {
            return string.Format("nesevents-{0}.{1}", bucketId, streamId);
        }

        public string CreateStreamCommits(string bucketId, string streamId)
        {
            return string.Format("NES.{0}.{1}.COMMITS", bucketId, streamId);
        }

        public string CreateStreamSnapshots(string bucketId, string streamId)
        {
            return string.Format("NES.{0}.{1}.SNAPSHOTS", bucketId, streamId);
        }

        public string CreateStreamsToSnapshot(string bucketId)
        {
            return string.Format("NES.{0}.STREAMSTOSNAPSHOT", bucketId);
        }
    }
}