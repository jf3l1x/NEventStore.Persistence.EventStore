using System;
using System.Linq;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Exceptions;
using EventStore.ClientAPI.SystemData;
using NEventStore.Logging;
using NEventStore.Persistence.EventStore.Projections;
using Newtonsoft.Json;

namespace NEventStore.Persistence.EventStore.Extensions
{
    public static class ProjectionManagerExtensions
    {
        private static readonly ILog Logger = LogFactory.BuildLogger(typeof(ProjectionManagerExtensions));
        public static ProjectionInfo GetProjectionInfo(this ProjectionsManager pm, string name,
            UserCredentials userCredentials)
        {
            string status = pm.GetStatusAsync(name, userCredentials).Result;
            return JsonConvert.DeserializeObject<ProjectionInfo>(status);
        }

        public static void EnableIfDisabled(this ProjectionsManager pm, string name,
            UserCredentials userCredentials)
        {
            if (!pm.GetProjectionInfo(name,userCredentials).IsEnabled)
            {
                Logger.Debug("assuming that projection {0} was disabled, enabling... ", name);
                pm.EnableAsync(name, userCredentials).Wait();
            }
            
        }
        public  static void RegisterQuery(this ProjectionsManager pm, string name, string query, UserCredentials credentials)
        {
            try
            {
                string existing = pm.GetQueryAsync(name, credentials).Result;
                if (existing!=query)
                {
                    Logger.Debug("projection {0} has changed, updating... ", name);
                    pm.UpdateQueryAsync(name, query, credentials).Wait();
                }
                
            }
            catch (AggregateException ae)
            {
                if (ae.InnerExceptions.Any(ie => ie is ProjectionCommandFailedException))
                {
                    var p = ae.InnerExceptions.First(ie => ie is ProjectionCommandFailedException);
                    Logger.Debug("assuming that projection {0} does not exists, creating... ", name);
                    pm.CreateContinuousAsync(name, query, credentials).Wait();
                }


            }
            pm.EnableIfDisabled(name, credentials);

        }

    }
}