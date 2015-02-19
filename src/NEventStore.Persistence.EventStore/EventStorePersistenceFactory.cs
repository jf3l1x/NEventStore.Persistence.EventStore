using NEventStore.Persistence.EventStore.Services;

namespace NEventStore.Persistence.EventStore
{
    public class EventStorePersistenceFactory : IPersistenceFactory
    {
        private readonly IStreamNamingStrategy _namingStrategy;
        private readonly EventStorePersistenceOptions _options;
        private readonly IEventStoreSerializer _serializer;

        public EventStorePersistenceFactory(EventStorePersistenceOptions options, IEventStoreSerializer serializer,
            IStreamNamingStrategy namingStrategy)
        {
            _options = options;
            _serializer = serializer;
            _namingStrategy = namingStrategy;
        }

        public IPersistStreams Build()
        {
            return new EventStorePersistenceEngine(_options.CreateConnection(), _serializer, _namingStrategy, _options);
        }
    }
}