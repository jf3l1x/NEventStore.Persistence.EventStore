using System;

namespace NEventStore.Persistence.GES.Events
{
    public class EventBase : IEvent
    {
        public EventBase()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
    }
}