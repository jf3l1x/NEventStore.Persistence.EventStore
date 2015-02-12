using System;
using EventStore.ClientAPI;
using NEventStore.Serialization;

namespace NEventStore.Persistence.GES.Extensions
{
    public static class EventStoreConnectionExtensions
    {
        public static void ActOnAll<T>(this IEventStoreConnection connection, string streamId, Action<T> action,
            IEventStoreSerializer serializer)
        {
            StreamEventsSlice currentSlice;
            int nextSliceStart = StreamPosition.Start;
            do
            {
                currentSlice = connection.ReadStreamEventsForwardAsync(streamId, nextSliceStart, 200, false).Result;
                nextSliceStart = currentSlice.NextEventNumber;
                foreach (ResolvedEvent resolvedEvent in currentSlice.Events)
                {
                    action((T)serializer.Deserialize(typeof(T).FullName,resolvedEvent.Event.Data));
                }
            } while (!currentSlice.IsEndOfStream);
        }
    }
}