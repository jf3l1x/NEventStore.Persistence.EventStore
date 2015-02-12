using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using EventStore.ClientAPI;
using FluentAssertions;
using NEventStore.Persistence.GES.Tests.Extensions;
using NEventStore.Serialization;
using TechTalk.SpecFlow;

namespace NEventStore.Persistence.GES.Tests.Steps
{
    [Binding]
    [Scope(Feature = "Persistence")]
    internal class PersistenceSteps
    {
        public PersistenceSteps()
        {
            ScenarioContext.Current.SetNewGuid(Keys.BucketId);
        }

        [Given(@"I have a PersistenceEngine")]
        public void GivenIHaveAPersistenceEngine()
        {
            IEventStoreConnection connection = EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113));
            connection.ConnectAsync().Wait();
            ScenarioContext.Current.Add(new GESPersistenceEngine(connection,new JsonNetSerializer()));
        }

        [Given(@"I have initiliazed the Engine")]
        public void GivenIHaveInitiliazedTheEngine()
        {
            ScenarioContext.Current.Get<GESPersistenceEngine>().Initialize();
        }

        [Given(@"The PersistentStore is empty")]
        public void GivenThePersistentStoreIsEmpty()
        {
            ScenarioContext.Current.Get<GESPersistenceEngine>().Purge();
        }


        [Given(@"I Commit a event with the following commit headers")]
        public void GivenICommitAEventWithTheFollowingCommitHeaders(Table table)
        {
            Dictionary<string, object> headers =
                table.Rows.ToDictionary<TableRow, string, object>(tableRow => tableRow["key"],
                    tableRow => tableRow["value"]);
            ScenarioContext.Current.Get<GESPersistenceEngine>().Commit(CreateCommitAttempt(headers));
        }

        private CommitAttempt CreateCommitAttempt(Dictionary<string, object> headers = null)
        {
            Guid commitId = ScenarioContext.Current.SetNewGuid(Keys.CommitId);
            string streamId = ScenarioContext.Current.SetNewGuid(Keys.StreamId).ToString("N");

            return  new CommitAttempt(
                ScenarioContext.Current.Get<Guid>(Keys.BucketId).ToString("N"),
                streamId,
                1,
                commitId,
                1,
                DateTime.UtcNow,
                headers,
                new[]
                {
                    new EventMessage
                    {
                        Body = "test",
                        Headers = new Dictionary<string, object>()
                    }
                });
        }

        [When(@"I Get all commits for the current Stream")]
        public void WhenIGetAllCommitsForTheCurrentStream()
        {
            ScenarioContext.Current.Add(ScenarioContext.Current.Get<GESPersistenceEngine>()
                .GetFrom(ScenarioContext.Current.Get<Guid>(Keys.BucketId).ToString("N"),
                    ScenarioContext.Current.Get<Guid>(Keys.StreamId).ToString("N"), 0, int.MaxValue));
        }

        [Then(@"the first commit should have the following headers")]
        public void ThenTheFirstCommitShouldHaveTheFollowingHeaders(Table table)
        {
            ICommit commit = ScenarioContext.Current.Get<IEnumerable<ICommit>>().First();
            foreach (TableRow tableRow in table.Rows)
            {
                commit.Headers[tableRow["key"]].Should().Be(tableRow["value"]);
            }
        }
    }
}