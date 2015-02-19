namespace NEventStore.Persistence.EventStore.Events
{
    public class SnapshotThresholdReached : EventBase
    {
        public string StreamId { get; set; }
    }
}