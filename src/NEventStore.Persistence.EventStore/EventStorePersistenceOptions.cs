using System.Net;
using EventStore.ClientAPI;

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
        public ConnectionSettings ConnectionSettings { get; set; }
        public ClusterSettings ClusterSettings { get; set; }
        public IPEndPoint Server { get; set; }

        public IEventStoreConnection CreateConnection()
        {
            if (ConnectionSettings != null)
            {
                if (ClusterSettings != null)
                {
                    return EventStoreConnection.Create(ConnectionSettings, ClusterSettings);
                }
                return EventStoreConnection.Create(ConnectionSettings, Server);
            }
            return EventStoreConnection.Create(Server);
        }
    }
}