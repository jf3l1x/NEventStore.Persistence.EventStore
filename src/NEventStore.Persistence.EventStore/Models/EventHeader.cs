using System;
using System.Collections.Generic;

namespace NEventStore.Persistence.EventStore.Models
{
    public class EventHeader
    {
        public EventHeader()
        {
            AdditionalHeaders = new Dictionary<string, object>();
        }

        public string BucketId { get; set; }
        public string StreamId { get; set; }
        public Guid CommitId { get; set; }
        public int CommitSequence { get; set; }
        public DateTime CommitStamp { get; set; }
        public int StreamRevision { get; set; }
        public Dictionary<string, object> AdditionalHeaders { get; set; }
    }
}