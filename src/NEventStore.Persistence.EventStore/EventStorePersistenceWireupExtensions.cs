using NEventStore.Persistence.EventStore.Services;

namespace NEventStore.Persistence.EventStore
{
    public static class EventStorePersistenceWireupExtensions
    {
        public static PersistenceWireup UsingEventStorePersistence(this Wireup wireup,
            EventStorePersistenceOptions options, IEventStoreSerializer serializer, IStreamNamingStrategy namingStrategy)
        {
            return new EventStorePersistenceWireup(wireup, options, serializer, namingStrategy);
        }
    }
}