using System;
using EventStore.ClientAPI;
using NEventStore.Persistence.EventStore.Services;

namespace NEventStore.Persistence.EventStore.Extensions
{
    public static class SnapShotExtensions
    {
        public static string GetStreamName(this ISnapshot snapshot,IStreamNamingStrategy strategy)
        {
            return strategy.CreateStreamSnapshots(snapshot.BucketId, snapshot.StreamId);
        }
        public static EventData ToEventData(this ISnapshot snapshot, IEventStoreSerializer serializer)
        {
            return new EventData(Guid.NewGuid(), snapshot.GetType().FullName, serializer.IsJsonSerializer, serializer.Serialize(snapshot), new byte[0]);
        }
        public static ISnapshot ToSnapshot(this RecordedEvent snapshot, IEventStoreSerializer serializer)
        {
            return serializer.Deserialize<Snapshot>( snapshot.Data);
        }
    }
}