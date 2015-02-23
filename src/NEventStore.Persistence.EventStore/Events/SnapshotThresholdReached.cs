namespace NEventStore.Persistence.EventStore.Events
{
    public class SnapshotThresholdReached 
    {
        public string StreamId { get; set; }
    }
}