namespace NEventStore.Persistence.EventStore.Events
{
    public class BucketCreated : EventBase
    {
        public string Bucket { get; set; }
    }
}