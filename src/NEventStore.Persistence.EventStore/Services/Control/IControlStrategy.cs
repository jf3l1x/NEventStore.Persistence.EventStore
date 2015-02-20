namespace NEventStore.Persistence.EventStore.Services.Control
{
    internal interface IControlStrategy
    {
        void Initialize();
        void PreProcessCommitAttempt(CommitAttempt attempt);
        void PostProcessCommitAttempt(CommitAttempt attempt);
    }
}
