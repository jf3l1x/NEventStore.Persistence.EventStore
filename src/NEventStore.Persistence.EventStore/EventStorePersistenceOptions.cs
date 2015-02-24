using System;
using System.Net;
using System.Text.RegularExpressions;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;

namespace NEventStore.Persistence.EventStore
{
    public class EventStorePersistenceOptions
    {
        public EventStorePersistenceOptions()
        {
            WritePageSize = 500;
            ReadPageSize = 500;
            UseProjections = false;
            ProjectionRegistrationTimeout = TimeSpan.FromMinutes(5);
        }

        public int WritePageSize { get; set; }
        public int ReadPageSize { get; set; }
        public bool UseProjections { get; set; }
        public int MinimunSnapshotThreshold { get; set; }
        public ConnectionSettings ConnectionSettings { get; set; }
        public ClusterSettings ClusterSettings { get; set; }
        public IPEndPoint TcpeEndPoint { get; set; }
        public IPEndPoint HttpEndPoint { get; set; }
        public UserCredentials UserCredentials { get; set; }
        public TimeSpan ProjectionRegistrationTimeout { get; set; }

        public string Evaluate(Match match)
        {
            var property = match.Groups["Option"].Value;
            if (!string.IsNullOrEmpty(property))
            {
                return GetType().GetProperty(property).GetValue(this).ToString();
            }
            return string.Empty;
        }
        public IEventStoreConnection CreateConnection()
        {
            if (ConnectionSettings != null)
            {
                if (ClusterSettings != null)
                {
                    return EventStoreConnection.Create(ConnectionSettings, ClusterSettings);
                }
                return EventStoreConnection.Create(ConnectionSettings, TcpeEndPoint);
            }
            return EventStoreConnection.Create(TcpeEndPoint);
        }
    }
}