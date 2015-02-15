using System;
using TechTalk.SpecFlow;

namespace NEventStore.Persistence.EventStore.Tests.Extensions
{
    internal static class ScenarioContextExtensions
    {
        internal static void Add<T>(this ScenarioContext context, T value)
        {
            context.Add(typeof (T).FullName, value);
        }

        internal static T Get<T>(this ScenarioContext context)
        {
            if (context.ContainsKey(typeof (T).FullName))
            {
                return context.Get<T>(typeof (T).FullName);
            }
            return default(T);
        }

        internal static Guid SetNewGuid(this ScenarioContext context,string key)
        {
            var result = Guid.NewGuid();
            context.Set(result, key);
            return result;
        }
    }
}