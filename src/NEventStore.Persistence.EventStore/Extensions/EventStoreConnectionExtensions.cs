using System;
using System.Linq;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using NEventStore.Persistence.EventStore.Services;

namespace NEventStore.Persistence.EventStore.Extensions
{
    public static class EventStoreConnectionExtensions
    {
        public static void ActOnAll<T>(this IEventStoreConnection connection, string streamId, Action<T> action,
            IEventStoreSerializer serializer,UserCredentials credentials)
        {
            StreamEventsSlice currentSlice;
            int nextSliceStart = StreamPosition.Start;
            do
            {
                currentSlice = connection.ReadStreamEventsForwardAsync(streamId, nextSliceStart, 200, false,credentials).Result;
                nextSliceStart = currentSlice.NextEventNumber;
                foreach (ResolvedEvent resolvedEvent in currentSlice.Events)
                {
                    action((T)serializer.Deserialize(typeof(T).FullName,resolvedEvent.Event.Data));
                }
            } while (!currentSlice.IsEndOfStream);
        }

        public static T GetLast<T>(this IEventStoreConnection connection, string streamId, IEventStoreSerializer serializer, UserCredentials credentials)
        {
            StreamEventsSlice currentSlice = connection.ReadStreamEventsBackwardAsync(streamId, StreamPosition.End, 1, true, credentials).Result;
            if (currentSlice.Events.Any())
            {
                return serializer.Deserialize<T>(currentSlice.Events[0].Event.Data);
            }
            return default (T);
        }
    }
}