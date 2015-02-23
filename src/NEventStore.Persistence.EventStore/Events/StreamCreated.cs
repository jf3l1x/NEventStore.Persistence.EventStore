namespace NEventStore.Persistence.EventStore.Events
{
    public class StreamCreated
    {
        public string StreamId { get; set; }
        public string BucketId { get; set; }
    }
}