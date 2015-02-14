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
        private ICommit FirstCommit
        {
            get { return ScenarioContext.Current.Get<IEnumerable<ICommit>>().First(); }
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

        [Given(@"that i have a CommitAttemptGenerator")]
        public void GivenThatIHaveACommitAttemptGenerator()
        {
            ScenarioContext.Current.Add(new CommitAttemptGenerator(ScenarioContext.Current.Get<Guid>(Keys.BucketId),
                ScenarioContext.Current.Get<Guid>(Keys.StreamId)));
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
            FirstCommit.StreamId.Should()
                .Be(ScenarioContext.Current.Get<CommitAttemptGenerator>().Flushed.First().StreamId);
        }

        [Then(@"The first commit should have the same stream revision of the commit attempt")]
        public void ThenTheFirstCommitShouldHaveTheSameStreamRevisionOfTheCommitAttempt()
        {
            FirstCommit.StreamRevision.Should()
                .Be(ScenarioContext.Current.Get<CommitAttemptGenerator>().Flushed.First().StreamRevision);
        }

        [Then(@"The first commit should have the same commit sequence of the commit attempt")]
        public void ThenTheFirstCommitShouldHaveTheSameCommitSequenceOfTheCommitAttempt()
        {
            FirstCommit.CommitSequence.Should()
                .Be(ScenarioContext.Current.Get<CommitAttemptGenerator>().Flushed.First().CommitSequence);
        }

        [Then(@"The first commit should have the same number of headers of the commit attempt")]
        public void ThenTheFirstCommitShouldHaveTheSameNumberOfHeadersOfTheCommitAttempt()
        {
            FirstCommit.Headers.Count.Should()
                .Be(ScenarioContext.Current.Get<CommitAttemptGenerator>().Flushed.First().Headers.Count);
        }

        [Then(@"The first commit should have the same number of events of the commit attempt")]
        public void ThenTheFirstCommitShouldHaveTheSameNumberOfEventsOfTheCommitAttempt()
        {
            FirstCommit.Events.Count.Should()
                .Be(ScenarioContext.Current.Get<CommitAttemptGenerator>().Flushed.First().Events.Count);
        }

        [Then(@"The first commit should have a commit stamp within (.*) seconds of the commit attemp stamp")]
        public void ThenTheFirstCommitShouldHaveACommitStampWithinSecondsOfTheCommitAttempStamp(int seconds)
        {
            TimeSpan difference =
                FirstCommit.CommitStamp.Subtract(
                    ScenarioContext.Current.Get<CommitAttemptGenerator>().Flushed.First().CommitStamp);
            difference.Days.Should().Be(0);
            difference.Hours.Should().Be(0);
            difference.Minutes.Should().Be(0);
            difference.Seconds.Should().BeLessOrEqualTo(seconds);
        }

        [Given(@"I Have following commit attemps that was commited in this order")]
        public void GivenIHaveFollowingCommitAttempsThatWasCommitedInThisOrder(Table table)
        {
            foreach (TableRow tableRow in table.Rows)
            {
                ScenarioContext.Current.Get<CommitAttemptGenerator>().AddInfo(new CommitAttemptGenerationInfo
                {
                    Order = int.Parse(tableRow["Order"]),
                    EventCount = int.Parse(tableRow["EventCount"]),
                    CommitId = Guid.Parse(tableRow["CommitId"]),
                    StreamId = Guid.Parse(tableRow["StreamId"]),
                    BucketId = ScenarioContext.Current.Get<Guid>(Keys.BucketId)
                });
            }
            ScenarioContext.Current.Get<CommitAttemptGenerator>()
                .Flush(ScenarioContext.Current.Get<GESPersistenceEngine>());
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
            if (!string.IsNullOrEmpty(commitId))
            {
                ScenarioContext.Current.Get<IEnumerable<ICommit>>().Skip(1).First().CommitId.Should().Be(commitId);
            }
        }

        [Then(@"the current Exception should be of type ""(.*)""")]
        public void ThenTheCurrentExceptionShouldBeOfType(string typeName)
        {
            var exception = ScenarioContext.Current.Get<Exception>(Keys.CurrentException);
            exception.Should().NotBeNull();
            exception.GetType().FullName.Should().Be(typeName);
        }

        [Given(@"I Have (.*) commitAttemps with the same CommitSequence")]
        public void GivenIHaveCommitAttempsWithTheSameCommitSequence(int quantity)
        {
            ScenarioContext.Current.Get<CommitAttemptGenerator>().AddWithSameCommitSequence(quantity);
        }

        [Given(@"I Have (.*) commitAttemps with the same expected version")]
        public void GivenIHaveCommitAttempsWithTheSameExpectedVersion(int quantity)
        {
            ScenarioContext.Current.Get<CommitAttemptGenerator>().AddWithSameStreamRevision(quantity);
        }

        [When(@"I Commit all the commit attemps")]
        public void WhenICommitAllTheCommitAttemps()
        {
            try
            {
                ScenarioContext.Current.Get<CommitAttemptGenerator>()
                    .Flush(ScenarioContext.Current.Get<GESPersistenceEngine>());
            }
            catch (Exception ex)
            {
                ScenarioContext.Current.Set(ex, Keys.CurrentException);
            }
        }

        [Given(@"I Have defined a default bucket")]
        public void GivenIHaveDefinedADefaultBucket()
        {
            ScenarioContext.Current.Add(Keys.BucketId, Guid.NewGuid());
        }

        [Given(@"that i have defined a default stream")]
        public void GivenThatIHaveDefinedADefaultStream()
        {
            ScenarioContext.Current.Add(Keys.StreamId, Guid.NewGuid());
        }

        [Given(@"I Have (.*) commit attempt with the following headers")]
        public void GivenIHaveCommitAttemptWithTheFollowingHeaders(int count, Table table)
        {
            var headers = new Dictionary<string, object>();
            foreach (TableRow tableRow in table.Rows)
            {
                headers.Add(tableRow["key"], tableRow["value"]);
            }
            ScenarioContext.Current.Get<CommitAttemptGenerator>().AddInfo(count, headers);
        }

        [Given(@"I Have (.*) commit attempt")]
        public void GivenIHaveCommitAttempt(int count)
        {
            ScenarioContext.Current.Get<CommitAttemptGenerator>().AddInfo(count);
        }


        [Then(@"The first commit should have (.*) headers")]
        public void ThenTheFirstCommitShouldHaveHeaders(int count)
        {
            FirstCommit.Headers.Count.Should().Be(count);
        }

        [Given(@"I Have the same commit attempt (.*) times")]
        public void GivenIHaveTheSameCommitAttemptTimes(int count)
        {
            var info=ScenarioContext.Current.Get<CommitAttemptGenerator>().CreateDefault();
            info.IncrementCommitSequence = false;
            info.IncrementStreamRevision = false;
            for (int i = 0; i < count; i++)
            {
                ScenarioContext.Current.Get<CommitAttemptGenerator>().AddInfo(info);
            }
            
        }

        private class CommitAttemptGenerationInfo
        {
            public CommitAttemptGenerationInfo()
            {
                BucketId = Guid.NewGuid();
                CommitId = Guid.NewGuid();
                StreamId = Guid.NewGuid();
                EventCount = 0;
                Headers = new Dictionary<string, object>();
                IncrementCommitSequence = true;
                IncrementStreamRevision = true;
            }

            public int Order { get; set; }
            public Guid BucketId { get; set; }
            public Guid CommitId { get; set; }
            public Guid StreamId { get; set; }
            public int EventCount { get; set; }
            public bool IncrementCommitSequence { get; set; }
            public bool IncrementStreamRevision { get; set; }
            public Dictionary<string, object> Headers { get; set; }

            public IEnumerable<EventMessage> GenerateEvents()
            {
                var events = new List<EventMessage>();
                for (int i = 0; i < EventCount; i++)
                {
                    events.Add(new EventMessage
                    {
                        Body = "test" + i,
                        Headers = new Dictionary<string, object>()
                    });
                }
                return events;
            }
        }

        private class CommitAttemptGenerator
        {
            private readonly List<CommitAttemptGenerationInfo> _attempts = new List<CommitAttemptGenerationInfo>();
            private readonly SequenceGenerator _commitSequence = new SequenceGenerator(1);
            private readonly Guid _defaultBucketId;
            private readonly Guid _defaultStreamId;
            private readonly List<CommitAttempt> _flushed = new List<CommitAttempt>();
            private readonly SequenceGenerator _streamRevision = new SequenceGenerator(0);
            private int _order;

            public CommitAttemptGenerator(Guid defaultBucketId, Guid defaultStreamId)
            {
                _defaultBucketId = defaultBucketId;
                _defaultStreamId = defaultStreamId;
            }

            public IEnumerable<CommitAttempt> Flushed
            {
                get { return _flushed.ToArray(); }
            }

            public void AddInfo(CommitAttemptGenerationInfo info)
            {
                _attempts.Add(info);
                if (info.Order > _order)
                {
                    _order = info.Order;
                }
            }

            public void AddInfo(int count)
            {
                for (int i = 0; i < count; i++)
                {
                    AddInfo(CreateDefault());
                }
            }

            public void AddWithSameCommitSequence(int count)
            {
                for (int i = 0; i < count; i++)
                {
                    CommitAttemptGenerationInfo info = CreateDefault();
                    info.IncrementCommitSequence = false;
                    AddInfo(info);
                }
            }

            public void AddWithSameStreamRevision(int count)
            {
                for (int i = 0; i < count; i++)
                {
                    CommitAttemptGenerationInfo info = CreateDefault();
                    
                    info.IncrementStreamRevision = false;
                    AddInfo(info);
                }
            }

            public void AddInfo(int count, Dictionary<string, object> headers)
            {
                for (int i = 0; i < count; i++)
                {
                    CommitAttemptGenerationInfo info = CreateDefault();
                    info.Headers = headers;
                    AddInfo(info);
                }
            }

            public CommitAttemptGenerationInfo CreateDefault()
            {
                return new CommitAttemptGenerationInfo
                {
                    BucketId = _defaultBucketId,
                    CommitId = Guid.NewGuid(),
                    EventCount = 1,
                    Order = _order++,
                    StreamId = _defaultStreamId
                };
            }

            public void Flush(GESPersistenceEngine engine)
            {
                foreach (CommitAttemptGenerationInfo attempt in _attempts.OrderBy(a => a.Order))
                {
                   
                    if (attempt.IncrementStreamRevision || _streamRevision.ViewCurrent()==0)
                    {
                        _streamRevision.Increment(attempt.EventCount);
                    }
                    var commitAttempt = new CommitAttempt(attempt.BucketId.ToString("N"), attempt.StreamId.ToString("N"),
                        _streamRevision.ViewCurrent(), attempt.CommitId, _commitSequence.ViewCurrent(),
                        DateTime.UtcNow, attempt.Headers,
                        attempt.GenerateEvents());
                    engine.Commit(commitAttempt);
                    _flushed.Add(commitAttempt);
                    if (attempt.IncrementCommitSequence)
                    {
                        _commitSequence.Increment(1);
                    }
                }
                _attempts.Clear();
            }

           
        }

        private class SequenceGenerator
        {
            private readonly int? _fixedValue;
            private int _sequence;

            public SequenceGenerator(int start = 0, int? fixedValue = null)
            {
                _fixedValue = fixedValue;
                _sequence = start;
            }

            public int ConsumeCurrent()
            {
                int retval = ViewCurrent();
                _sequence++;
                return retval;
            }

            public int ViewCurrent()
            {
                if (_fixedValue.HasValue)
                {
                    return _fixedValue.Value;
                }
                return _sequence;
            }

            public void Increment(int count)
            {
                _sequence += count;
            }
        }
    }
}