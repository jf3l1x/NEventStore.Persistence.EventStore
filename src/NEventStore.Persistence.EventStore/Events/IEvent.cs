using System;

namespace NEventStore.Persistence.GES.Events
{
    public interface IEvent
    {
        Guid Id { get; set; }
    }
}