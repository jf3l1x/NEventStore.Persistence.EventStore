using NEventStore.Persistence.EventStore.Services;
using NEventStore.Persistence.EventStore.Services.Naming;

namespace NEventStore.Persistence.EventStore
{
    public class EventStorePersistenceFactory : IPersistenceFactory
    {
        private readonly IStreamNamingStrategy _namingStrategy;
        private readonly bool _useProjections;
        private readonly EventStorePersistenceOptions _options;
        private readonly IEventStoreSerializer _serializer;

        public EventStorePersistenceFactory(EventStorePersistenceOptions options, IEventStoreSerializer serializer,
            IStreamNamingStrategy namingStrategy,bool useProjections)
        {
            _options = options;
            _serializer = serializer;
            _namingStrategy = namingStrategy;
            _useProjections = useProjections;
        }

        public IPersistStreams Build()
        {
            return new EventStorePersistenceEngine(_options.CreateConnection(), _serializer, _namingStrategy, _options,_useProjections);
        }
    }
}