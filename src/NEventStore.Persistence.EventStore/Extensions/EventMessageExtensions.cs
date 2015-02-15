using EventStore.ClientAPI;
using NEventStore.Persistence.EventStore.Events;
using NEventStore.Persistence.EventStore.Services;

namespace NEventStore.Persistence.EventStore.Extensions
{
    public static class EventMessageExtensions
    {
        public static EventData ToEventData(this IEvent message, IEventStoreSerializer serializer)
        {
            var serialized = serializer.Serialize(message);
            //For now let`s assume all strings are json
            return new EventData(message.Id,message.GetType().ToString(),serializer.IsJsonSerializer,serialized,new byte[0]);
        }
    }
}