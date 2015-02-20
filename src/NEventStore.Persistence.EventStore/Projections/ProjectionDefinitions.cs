using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NEventStore.Persistence.EventStore.Projections
{
    public static class ProjectionDefinitions
    {
        public static void RegisterProjections(EventStorePersistenceOptions options)
        {
            
        }

        private static string ReadFromEmbeddedResource(string name)
        {
            using (var stream = typeof(ProjectionDefinitions).Assembly.GetManifestResourceStream(
                "NEventStore.Persistence.EventStore.Projections."+ name+".js"))
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
