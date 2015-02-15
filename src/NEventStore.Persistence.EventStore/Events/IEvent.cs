using System;

namespace NEventStore.Persistence.EventStore.Events
{
    public interface IEvent
    {
        Guid Id { get; set; }
    }
}