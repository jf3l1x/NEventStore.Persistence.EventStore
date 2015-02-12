using System;

namespace NEventStore.Persistence.GES.Events
{
    public class StreamCreated:EventBase
    {
        public string StreamId { get; set; }
        public string BucketId { get; set; }
    }
}