using System;
using System.Collections.Generic;
using System.Linq;
using EventStore.ClientAPI;
using NEventStore.Persistence.EventStore.Services;

namespace NEventStore.Persistence.EventStore.Models
{
    public class PersistentEvent
    {

        private readonly EventHeader _header;
        private readonly EventMessage _message;
        private readonly string _type;

        public PersistentEvent(EventMessage message, CommitAttempt attempt)
        {
            _message = message;
            _header=new EventHeader
            {
                BucketId = attempt.BucketId,
                StreamId = attempt.StreamId,
                CommitId = attempt.CommitId,
                CommitSequence = attempt.CommitSequence,
                CommitStamp = attempt.CommitStamp,
                StreamRevision = attempt.StreamRevision
            };
            AddHeaders(message.Headers);
            AddCommitHeaders(attempt);
            _type = message.Body.GetType().FullName;
            
        }

        public Dictionary<string, object> GetCommitHeaders()
        {
            return _header.AdditionalHeaders.Where(kvp => kvp.Key.StartsWith(Headers.CommitHeaders))
                .ToDictionary(kvp => kvp.Key.Substring(Headers.CommitHeaders.Length+1), kvp => kvp.Value);
        }
        public PersistentEvent(ResolvedEvent evt, IEventStoreSerializer serializer)
        {
            _header =
                (EventHeader)
                    serializer.Deserialize(typeof(EventHeader).FullName, evt.Event.Metadata);
            _message = new EventMessage
            {
                Body = serializer.Deserialize(evt.Event.EventType, evt.Event.Data),
                Headers = _header.AdditionalHeaders
            };
        }
        public int StreamRevision
        {
            get { return _header.StreamRevision; }
        }
        public int CommitSequence
        {
            get { return _header.CommitSequence; }
        }
        public Guid CommitId
        {
            get { return _header.CommitId; }
        }

        public DateTime CommitStamp
        {
            get { return _header.CommitStamp; }
        }

        public string BucketId
        {
            get { return _header.BucketId; }
        }

        private void AddHeaders(Dictionary<string, object> values)
        {
            foreach (var header in values)
            {
                _header.AdditionalHeaders.Add(header.Key, header.Value);
            }
        }

        private void AddCommitHeaders(CommitAttempt attempt)
        {
            foreach (var header in attempt.Headers)
            {
                _header.AdditionalHeaders.Add(string.Format("{0}.{1}", Headers.CommitHeaders, header.Key), header.Value);
            }
        }

   

        public EventMessage ToEventMessage()
        {
            return _message;
        }

        public EventData ToEventData(IEventStoreSerializer serializer)
        {
            return new EventData(Guid.NewGuid(), _type, serializer.IsJsonSerializer, serializer.Serialize(_message.Body),
                serializer.Serialize(_header));
        }
        
    }
}