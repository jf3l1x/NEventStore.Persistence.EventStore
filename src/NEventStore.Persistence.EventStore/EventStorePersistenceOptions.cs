namespace NEventStore.Persistence.EventStore
{
    public class EventStorePersistenceOptions
    {
        public EventStorePersistenceOptions()
        {
            WritePageSize = 500;
            ReadPageSize = 500;
            UseProjections = false;
        }

        public int WritePageSize { get; set; }
        public int ReadPageSize { get; set; }
        public bool UseProjections { get; set; }
        public int MinimunSnapshotThreshold { get; set; }
    }
}