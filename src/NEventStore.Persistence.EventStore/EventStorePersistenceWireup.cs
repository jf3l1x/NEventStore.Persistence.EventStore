using NEventStore.Logging;
using NEventStore.Persistence.EventStore.Services;

namespace NEventStore.Persistence.EventStore
{
    public class EventStorePersistenceWireup : PersistenceWireup
    {
        private static readonly ILog Logger = LogFactory.BuildLogger(typeof (EventStorePersistenceWireup));

        public EventStorePersistenceWireup(Wireup inner, EventStorePersistenceOptions persistenceOptions,
            IEventStoreSerializer serializer, IStreamNamingStrategy namingStrategy)
            : base(inner)
        {
            Logger.Debug("Configuring EventStore persistence engine.");
            Container.Register(
                c => new EventStorePersistenceFactory(persistenceOptions, serializer, namingStrategy).Build());
        }
    }
}