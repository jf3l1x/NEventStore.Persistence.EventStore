using System;
using EventStore.ClientAPI;
using NEventStore.Persistence.EventStore.Events;
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
        public static EventData ToEventData(this ISnapshot snapshot, IEventStoreSerializer serializer)
        {
            return new EventData(Guid.NewGuid(), snapshot.GetType().FullName, serializer.IsJsonSerializer, serializer.Serialize(snapshot), new byte[0]);
        }
        public static ISnapshot ToSnapshot(this RecordedEvent snapshot, IEventStoreSerializer serializer)
        {
            return serializer.Deserialize<Snapshot>( snapshot.Data);
        }
        public static StreamHeadChanged CreateEventSnapshotCreated(this ISnapshot snapshot)
        {
            return new StreamHeadChanged()
            {
                BucketId = snapshot.BucketId,
                StreamId = snapshot.StreamId,
                SnapshotRevision = snapshot.StreamRevision
            };
        }
    }
}