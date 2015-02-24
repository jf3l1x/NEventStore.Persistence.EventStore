using System.IO;
using System.Text.RegularExpressions;
using EventStore.ClientAPI;
using NEventStore.Logging;
using NEventStore.Persistence.EventStore.Extensions;
using NEventStore.Persistence.EventStore.Services;

namespace NEventStore.Persistence.EventStore.Projections
{
    public static class ProjectionDefinitions
    {
        private static readonly ILog Logger = LogFactory.BuildLogger(typeof(ProjectionDefinitions));
        private static Regex _parameters=new Regex(@"\{\{(?<Option>[^}]+)\}\}",RegexOptions.Compiled|RegexOptions.IgnoreCase);

        public static void RegisterProjections(EventStorePersistenceOptions options)
        {
            var pm = new ProjectionsManager(new EventStoreLogger(Logger), options.HttpEndPoint,
                options.ProjectionRegistrationTimeout);
            pm.EnableIfDisabled("$by_category", options.UserCredentials);
            pm.RegisterQuery("nes.events", ReadFromEmbeddedResource("Events",options), options.UserCredentials);
            pm.RegisterQuery("nes.snapshots", ReadFromEmbeddedResource("Snapshots", options), options.UserCredentials);
            pm.RegisterQuery("nes.streamsToSnapshot", ReadFromEmbeddedResource("StreamsToSnapshot", options), options.UserCredentials);
            
        }
        
        
        private static string ReadFromEmbeddedResource(string name,EventStorePersistenceOptions options)
        {
            using (Stream stream = typeof (ProjectionDefinitions).Assembly.GetManifestResourceStream(
                "NEventStore.Persistence.EventStore.Projections." + name + ".js"))
            {
                var reader = new StreamReader(stream);
                return _parameters.Replace(reader.ReadToEnd(),options.Evaluate);
            }
        }

       
    }
}