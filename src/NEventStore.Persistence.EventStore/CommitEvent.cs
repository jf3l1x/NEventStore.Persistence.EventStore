using System;
using System.Collections.Generic;
using System.Linq;
using EventStore.ClientAPI;

namespace NEventStore.Persistence.GES
{
    public class CommitEvent
    {
        private readonly CommitAttempt _attempt;
        private readonly EventMessage _message;
        private readonly Dictionary<string, object> _headers;
        private readonly string _type;

        public CommitEvent(EventMessage message, CommitAttempt attempt)
        {
            _message = message;
            _attempt = attempt;
            
            _headers = new Dictionary<string, object>(_message.Headers)
            {
                {Headers.CommitId, attempt.CommitId},
                {Headers.CommitStamp, attempt.CommitStamp},
                {Headers.BucketId, attempt.BucketId},
                {Headers.CommitHeaders, attempt.Headers}
            };
            _type = message.Body.GetType().FullName;
        }

        public Guid CommitId
        {
            get { return (Guid)_headers[Headers.CommitId]; }
        }

        public DateTime CommitStamp
        {
            get { return (DateTime) _headers[Headers.CommitStamp]; }
        }
        public Dictionary<string, object> CommitHeaders
        {
            get { return (Dictionary<string,object>)_headers[Headers.CommitHeaders]; }
        }
        public Guid BucketId
        {
            get { return (Guid) _headers[Headers.BucketId]; }
        }
        public CommitEvent(ResolvedEvent evt, IEventStoreSerializer serializer)
        {
            _headers = (Dictionary<string, object>)serializer.Deserialize(typeof (Dictionary<string, object>).FullName, evt.Event.Metadata);
            _message = new EventMessage()
            {
                Body = serializer.Deserialize(evt.Event.EventType, evt.Event.Data),
                Headers = _headers
            };
        }

        public EventMessage ToEventMessage()
        {
            return _message;
        }
        public EventData ToEventData(IEventStoreSerializer serializer)
        {
            return new EventData(Guid.NewGuid(),_type,serializer.IsJsonSerializer,serializer.Serialize(_message.Body),serializer.Serialize(_headers));
        }
    }
}