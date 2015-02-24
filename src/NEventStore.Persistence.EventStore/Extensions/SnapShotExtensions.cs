using System;
using EventStore.ClientAPI;
using NEventStore.Persistence.EventStore.Models;
using NEventStore.Persistence.EventStore.Services;
using NEventStore.Persistence.EventStore.Services.Naming;

namespace NEventStore.Persistence.EventStore.Extensions
{
    public static class SnapshotExtensions
    {
        public static string GetStreamName(this ISnapshot snapshot,IStreamNamingStrategy strategy)
        {
            return strategy.CreateStreamSnapshots(snapshot.BucketId, snapshot.StreamId);
        }
        public static EventData ToEventData(this ISnapshot snapshot, IEventStoreSerializer serializer,SnapshotMetadata metadata)
        {
            return new EventData(Guid.NewGuid(), snapshot.GetType().FullName, serializer.IsJsonSerializer, serializer.Serialize(snapshot), serializer.Serialize(metadata));
        }
        public static ISnapshot ToSnapshot(this RecordedEvent snapshot, IEventStoreSerializer serializer)
        {
            return serializer.Deserialize<Snapshot>( snapshot.Data);
        }
    }
}