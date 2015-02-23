using NEventStore.Persistence.EventStore.Projections;

namespace NEventStore.Persistence.EventStore.Services.Control
{
    internal class UseProjectionsStrategy : IControlStrategy
    {
        private static bool _isInitialized;
        private readonly object _lockObject = new object();
        private readonly EventStorePersistenceOptions _options;

        public UseProjectionsStrategy(EventStorePersistenceOptions options)
        {
            _options = options;
        }

        public void Initialize()
        {

            lock (_lockObject)
            {
                if (!_isInitialized)
                {
                    ProjectionDefinitions.RegisterProjections(_options);
                    _isInitialized = true;
                }
            }

        }

        public void PreProcessCommitAttempt(CommitAttempt attempt)
        {
        }

        public void PostProcessCommitAttempt(CommitAttempt attempt)
        {
        }
    }
}