using System.IO;
using EventStore.ClientAPI;
using NEventStore.Logging;
using NEventStore.Persistence.EventStore.Extensions;
using NEventStore.Persistence.EventStore.Services;

namespace NEventStore.Persistence.EventStore.Projections
{
    public static class ProjectionDefinitions
    {
        private static readonly ILog Logger = LogFactory.BuildLogger(typeof(ProjectionDefinitions));

        public static void RegisterProjections(EventStorePersistenceOptions options)
        {
            var pm = new ProjectionsManager(new EventStoreLogger(Logger), options.HttpEndPoint,
                options.ProjectionRegistrationTimeout);
            pm.EnableIfDisabled("$by_category", options.UserCredentials);
            pm.RegisterQuery("Buckets", BucketProjection(), options.UserCredentials);
        }


        private static string ReadFromEmbeddedResource(string name)
        {
            using (Stream stream = typeof (ProjectionDefinitions).Assembly.GetManifestResourceStream(
                "NEventStore.Persistence.EventStore.Projections." + name + ".js"))
            {
                var reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
        }

        private static string BucketProjection()
        {
            return ReadFromEmbeddedResource("Buckets");
        }
    }
}