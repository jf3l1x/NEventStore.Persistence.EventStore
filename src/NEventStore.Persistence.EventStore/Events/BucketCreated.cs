using System;

namespace NEventStore.Persistence.GES.Events
{
    public class BucketCreated : EventBase
    {
        public string Bucket { get; set; }
    }
}