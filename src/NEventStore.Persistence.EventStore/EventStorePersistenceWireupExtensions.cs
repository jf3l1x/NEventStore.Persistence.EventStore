using NEventStore.Persistence.EventStore.Services;
using NEventStore.Persistence.EventStore.Services.Naming;

namespace NEventStore.Persistence.EventStore
{
    public static class EventStorePersistenceWireupExtensions
    {
        public static PersistenceWireup UsingEventStorePersistence(this Wireup wireup,
            EventStorePersistenceOptions options, IEventStoreSerializer serializer, IStreamNamingStrategy namingStrategy)
        {
            return new EventStorePersistenceWireup(wireup, options, serializer, namingStrategy,false);
        }
        public static PersistenceWireup UsingEventStorePersistenceWithProjections(this Wireup wireup,
           EventStorePersistenceOptions options, IEventStoreSerializer serializer, IStreamNamingStrategy namingStrategy)
        {
            return new EventStorePersistenceWireup(wireup, options, serializer, namingStrategy,true);
        }
    }
}