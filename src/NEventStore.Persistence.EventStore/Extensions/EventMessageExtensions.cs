using System;
using System.Collections.Generic;
using System.Text;
using EventStore.ClientAPI;
using NEventStore.Persistence.GES.Events;
using NEventStore.Persistence.GES.Services;
using NEventStore.Serialization;

namespace NEventStore.Persistence.GES.Extensions
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