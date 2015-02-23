using System;
using EventStore.ClientAPI;
using NEventStore.Persistence.EventStore.Events;
using NEventStore.Persistence.EventStore.Services;

namespace NEventStore.Persistence.EventStore.Extensions
{
    public static class EventMessageExtensions
    {
        public static EventData ToEventData(this object message, IEventStoreSerializer serializer)
        {
            var serialized = serializer.Serialize(message);
            return new EventData(Guid.NewGuid(),message.GetType().ToString(),serializer.IsJsonSerializer,serialized,new byte[0]);
        }
    }
}