using System;
using EventStore.ClientAPI;

namespace NEventStore.Persistence.EventStore.Services.Control
{
    internal class UseProjectionsStrategy : IControlStrategy
    {
        private readonly EventStorePersistenceOptions _options;

        public UseProjectionsStrategy( EventStorePersistenceOptions options)
        {
            _options = options;
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void PreProcessCommitAttempt(CommitAttempt attempt)
        {
            throw new NotImplementedException();
        }

        public void PostProcessCommitAttempt(CommitAttempt attempt)
        {
            throw new NotImplementedException();
        }

    }
}