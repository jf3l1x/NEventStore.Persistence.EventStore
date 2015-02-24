namespace NEventStore.Persistence.EventStore.Events
{
    public class StreamHeadChanged
    {
        public string BucketId { get; set; }
        public string StreamId { get; set; }
        public int HeadRevision { get; set; }
        public int SnapshotRevision { get; set; }

    }
}