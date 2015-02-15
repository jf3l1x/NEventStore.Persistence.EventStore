namespace NEventStore.Persistence.EventStore.Events
{
    public class StreamCreated:EventBase
    {
        public string StreamId { get; set; }
        public string BucketId { get; set; }
    }
}