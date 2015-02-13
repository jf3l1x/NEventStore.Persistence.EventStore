using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using EventStore.ClientAPI;
using FluentAssertions;
using NEventStore.Persistence.GES.Services;
using NEventStore.Persistence.GES.Tests.Extensions;
using TechTalk.SpecFlow;

namespace NEventStore.Persistence.GES.Tests.Steps
{
    [Binding]
    [Scope(Feature = "Persistence")]
    internal class PersistenceSteps
    {
        private class CommitAttemptGenerationInfo
        {
            public CommitAttemptGenerationInfo()
            {
                BucketId = Guid.NewGuid();
                CommitId = Guid.NewGuid();
                StreamId = Guid.NewGuid();
                EventCount = 0;
                Headers=new Dictionary<string, object>();
            }

            public int Order { get; set; }
            public Guid BucketId { get; set; }
            public Guid CommitId { get; set; }
            public Guid StreamId { get; set; }
            public int EventCount { get; set; }
            public Dictionary<string,object> Headers { get; set; }
            public IEnumerable<EventMessage> GenerateEvents()
            {
                return CreateEvents(EventCount);
                
            }

        }
        private class CommitAttemptGenerator
        {
            private readonly List<CommitAttemptGenerationInfo> _attempts=new List<CommitAttemptGenerationInfo>();
            public void AddInfo(CommitAttemptGenerationInfo info)
            {
                _attempts.Add(info);

            }

            public IEnumerable<CommitAttempt> Generate()
            {
                var commitAttempts=new List<CommitAttempt>();
                int streamRevision = 0;
                int commitSequence = 1;
                foreach (var attempt in _attempts.OrderBy(a=>a.Order))
                {
                    streamRevision += attempt.EventCount;
                    commitAttempts.Add(new CommitAttempt(attempt.BucketId.ToString("N"), attempt.StreamId.ToString("N"),
                        streamRevision, attempt.CommitId, commitSequence++, DateTime.UtcNow, attempt.Headers,
                        attempt.GenerateEvents()));
                }
                return commitAttempts;
            }
        }
        public PersistenceSteps()
        {
            ScenarioContext.Current.SetNewGuid(Keys.BucketId);
        }

        private ICommit FirstCommit
        {
            get { return ScenarioContext.Current.Get<IEnumerable<ICommit>>().First(); }
        }

        private CommitAttempt CommitAttempt
        {
            get { return ScenarioContext.Current.Get<CommitAttempt>(); }
        }

        [Given(@"I have a PersistenceEngine")]
        public void GivenIHaveAPersistenceEngine()
        {
            IEventStoreConnection connection = EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113));
            connection.ConnectAsync().Wait();
            ScenarioContext.Current.Add(new GESPersistenceEngine(connection, new JsonNetSerializer()));
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

        private CommitAttempt CreateCommitAttempt(Guid commitId, Guid streamId, int eventCount, Dictionary<string, object> headers = null)
        {
            if (headers == null)
            {
                headers = new Dictionary<string, object>();
            }
            ;
            return new CommitAttempt(
                ScenarioContext.Current.Get<Guid>(Keys.BucketId).ToString("N"),
                streamId.ToString("N"),
                1,
                commitId,
                1,
                DateTime.UtcNow,
                headers,
                CreateEvents(eventCount));
        }

        private static IEnumerable<EventMessage> CreateEvents(int eventCount)
        {
            var events = new List<EventMessage>();
            for (int i = 0; i < eventCount; i++)
            {
                events.Add(new EventMessage
                {
                    Body = "test" + i,
                    Headers = new Dictionary<string, object>()
                });
            }
            return events;
        }

        private CommitAttempt CreateCommitAttempt(Dictionary<string, object> headers = null)
        {
            var commitId = ScenarioContext.Current.SetNewGuid(Keys.CommitId);
            var streamId = ScenarioContext.Current.SetNewGuid(Keys.StreamId);
            return CreateCommitAttempt(commitId, streamId, 1, headers);
        }

        [Given(@"I Have a commit attempt")]
        public void GivenIHaveACommitAttempt()
        {
            ScenarioContext.Current.Add(CreateCommitAttempt());
        }

        [Given(@"I set the following headers in the commit attempt")]
        public void GivenISetTheFollowingHeadersInTheCommitAttempt(Table table)
        {
            var attempt = ScenarioContext.Current.Get<CommitAttempt>();
            foreach (TableRow tableRow in table.Rows)
            {
                attempt.Headers.Add(tableRow["key"], tableRow["value"]);
            }
        }

        [When(@"I Commit the commitAttempt")]
        public void WhenICommitTheCommitAttempt()
        {
            ScenarioContext.Current.Get<GESPersistenceEngine>().Commit(ScenarioContext.Current.Get<CommitAttempt>());
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
            foreach (TableRow tableRow in table.Rows)
            {
                FirstCommit.Headers[tableRow["key"]].Should().Be(tableRow["value"]);
            }
        }

        [Then(@"The first commit should have the same stream identifier of the commit attempt")]
        public void ThenTheFirstCommitShouldHaveTheSameStreamIdentifierOfTheCommitAttempt()
        {
            FirstCommit.StreamId.Should().Be(CommitAttempt.StreamId);
        }

        [Then(@"The first commit should have the same stream revision of the commit attempt")]
        public void ThenTheFirstCommitShouldHaveTheSameStreamRevisionOfTheCommitAttempt()
        {
            FirstCommit.StreamRevision.Should().Be(CommitAttempt.StreamRevision);
        }

        [Then(@"The first commit should have the same commit sequence of the commit attempt")]
        public void ThenTheFirstCommitShouldHaveTheSameCommitSequenceOfTheCommitAttempt()
        {
            FirstCommit.CommitSequence.Should().Be(CommitAttempt.CommitSequence);
        }

        [Then(@"The first commit should have the same number of headers of the commit attempt")]
        public void ThenTheFirstCommitShouldHaveTheSameNumberOfHeadersOfTheCommitAttempt()
        {
            FirstCommit.Headers.Count.Should().Be(CommitAttempt.Headers.Count);
        }

        [Then(@"The first commit should have the same number of events of the commit attempt")]
        public void ThenTheFirstCommitShouldHaveTheSameNumberOfEventsOfTheCommitAttempt()
        {
            FirstCommit.Events.Count.Should().Be(CommitAttempt.Events.Count);
        }
        [Then(@"The first commit should have a commit stamp within (.*) seconds of the commit attemp stamp")]
        public void ThenTheFirstCommitShouldHaveACommitStampWithinSecondsOfTheCommitAttempStamp(int seconds)
        {
            var difference = FirstCommit.CommitStamp.Subtract(CommitAttempt.CommitStamp);
            difference.Days.Should().Be(0);
            difference.Hours.Should().Be(0);
            difference.Minutes.Should().Be(0);
            difference.Seconds.Should().BeLessOrEqualTo(seconds);
        }
        [Given(@"I Have following commit attemps that was commited in this order")]
        public void GivenIHaveFollowingCommitAttempsThatWasCommitedInThisOrder(Table table)
        {
            var generator = new CommitAttemptGenerator();

            foreach (var tableRow in table.Rows)
            {
                generator.AddInfo(new CommitAttemptGenerationInfo()
                {
                    Order = int.Parse(tableRow["Order"]),
                    EventCount = int.Parse(tableRow["EventCount"]),
                    CommitId = Guid.Parse(tableRow["CommitId"]),
                    StreamId = Guid.Parse(tableRow["StreamId"]),
                    BucketId = ScenarioContext.Current.Get<Guid>(Keys.BucketId)
                    
                });
                
            }
            foreach (var commitAttempt in generator.Generate())
            {
                ScenarioContext.Current.Get<GESPersistenceEngine>().Commit(commitAttempt);
            }
            

        }

     

        [When(@"I Get all commits fro the Stream ""(.*)"" from revision (.*) to revision (.*)")]
        public void WhenIGetAllCommitsFroTheStreamFromRevisionToRevision(string streamId, int minVersion, int maxVersion)
        {
            ScenarioContext.Current.Add(ScenarioContext.Current.Get<GESPersistenceEngine>()
               .GetFrom(ScenarioContext.Current.Get<Guid>(Keys.BucketId).ToString("N"),
                   Guid.Parse(streamId).ToString("N"), minVersion, maxVersion));
        }

        [Then(@"There should be (.*) commits")]
        public void ThenThereShouldBeCommits(int commitCount)
        {
            ScenarioContext.Current.Get<IEnumerable<ICommit>>().Count().Should().Be(commitCount);
        }

        [Then(@"The First Commit should have the CommitId ""(.*)""")]
        public void ThenTheFirstCommitShouldHaveTheCommitId(string commitId)
        {
            FirstCommit.CommitId.Should().Be(commitId);
        }

        [Then(@"The Second Commit should have the CommitId ""(.*)""")]
        public void ThenTheSecondCommitShouldHaveTheCommitId(string commitId)
        {
            ScenarioContext.Current.Get<IEnumerable<ICommit>>().Skip(1).First().CommitId.Should().Be(commitId);
        }

    }
}