using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using FluentAssertions;
using NEventStore.Persistence.EventStore.Services;
using NEventStore.Persistence.EventStore.Services.Naming;
using NEventStore.Persistence.EventStore.Tests.Extensions;
using TechTalk.SpecFlow;

namespace NEventStore.Persistence.EventStore.Tests.Steps
{
    [Binding]
    internal class PersistenceSteps
    {
        private ICommit FirstCommit
        {
            get { return ScenarioContext.Current.Get<IEnumerable<ICommit>>().First(); }
        }

        [Given(@"I have a PersistenceEngine")]
        public void GivenIHaveAPersistenceEngine()
        {
            CreatePersistenceEngine();
        }

        private static void CreatePersistenceEngine(bool useProjections=false)
        {
            var options = ScenarioContext.Current.Get<EventStorePersistenceOptions>();
            options.TcpeEndPoint = new IPEndPoint(IPAddress.Loopback, 1113);
            options.HttpEndPoint = new IPEndPoint(IPAddress.Loopback, 2113);
            IEventStoreConnection connection = EventStoreConnection.Create(options.TcpeEndPoint);
            connection.ConnectAsync().Wait();
            ScenarioContext.Current.Add(new EventStorePersistenceEngine(connection, new JsonNetSerializer(),
                new DefaultNamingStrategy(),
                options));
        }

        [Given(@"I have a PersistenceEngine using projections")]
        public void GivenIHaveAPersistenceEngineUsingProjections()
        {
            CreatePersistenceEngine(true);
        }

        [Given(@"i have the following options")]
        public void GivenIHaveTheFollowingOptions(Table table)
        {
            ScenarioContext.Current.Add(new EventStorePersistenceOptions
            {
                WritePageSize = int.Parse(table.Rows[0]["WritePageSize"]),
                ReadPageSize = int.Parse(table.Rows[0]["ReadPageSize"]),
                MinimunSnapshotThreshold = int.Parse(table.Rows[0]["MinimunSnapshotThreshold"]),
                UserCredentials = table.Rows[0].ContainsKey("UserName") ? new UserCredentials(table.Rows[0]["UserName"], table.Rows[0]["Password"]) : null
            });
        }

        [Then(@"The Should be (.*) Events in the commits")]
        public void ThenTheShouldBeEventsInTheCommits(int count)
        {
            ScenarioContext.Current.Get<IEnumerable<ICommit>>().Sum(c => c.Events.Count).Should().Be(count);
        }

        [Given(@"I have initiliazed the Engine")]
        public void GivenIHaveInitiliazedTheEngine()
        {
            Engine.Initialize();
        }

        [Given(@"The PersistentStore is empty")]
        public void GivenThePersistentStoreIsEmpty()
        {
            Engine.Purge();
        }

        [Given(@"that i have a CommitAttemptGenerator")]
        public void GivenThatIHaveACommitAttemptGenerator()
        {
            ScenarioContext.Current.Add(new CommitAttemptGenerator(BucketId,
                StreamId));
        }

        [When(@"I Get all commits for the current Stream")]
        public void WhenIGetAllCommitsForTheCurrentStream()
        {
            ScenarioContext.Current.Add(Engine
                .GetFrom(BucketId.ToString("N"),
                    StreamId.ToString("N"), 0, int.MaxValue));
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
                .Be(Generator.Flushed.First().StreamId);
        }

        [Then(@"The first commit should have the same stream revision of the commit attempt")]
        public void ThenTheFirstCommitShouldHaveTheSameStreamRevisionOfTheCommitAttempt()
        {
            FirstCommit.StreamRevision.Should()
                .Be(Generator.Flushed.First().StreamRevision);
        }

        [Then(@"The first commit should have the same commit sequence of the commit attempt")]
        public void ThenTheFirstCommitShouldHaveTheSameCommitSequenceOfTheCommitAttempt()
        {
            FirstCommit.CommitSequence.Should()
                .Be(Generator.Flushed.First().CommitSequence);
        }

        [Then(@"The first commit should have the same number of headers of the commit attempt")]
        public void ThenTheFirstCommitShouldHaveTheSameNumberOfHeadersOfTheCommitAttempt()
        {
            FirstCommit.Headers.Count.Should()
                .Be(Generator.Flushed.First().Headers.Count);
        }

        [Then(@"The first commit should have the same number of events of the commit attempt")]
        public void ThenTheFirstCommitShouldHaveTheSameNumberOfEventsOfTheCommitAttempt()
        {
            FirstCommit.Events.Count.Should()
                .Be(Generator.Flushed.First().Events.Count);
        }

        [Then(@"The first commit should have a commit stamp within (.*) seconds of the commit attemp stamp")]
        public void ThenTheFirstCommitShouldHaveACommitStampWithinSecondsOfTheCommitAttempStamp(int seconds)
        {
            TimeSpan difference =
                FirstCommit.CommitStamp.Subtract(
                    Generator.Flushed.First().CommitStamp);
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
                Generator.AddInfo(new CommitAttemptGenerationInfo
                {
                    Order = int.Parse(tableRow["Order"]),
                    EventCount = int.Parse(tableRow["EventCount"]),
                    CommitId =
                        string.IsNullOrEmpty(tableRow["CommitId"]) ? Guid.NewGuid() : Guid.Parse(tableRow["CommitId"]),
                    StreamId = string.IsNullOrEmpty(tableRow["StreamId"]) ? StreamId : Guid.Parse(tableRow["StreamId"]),
                    BucketId = BucketId
                });
            }
            Generator
                .Flush(Engine);
        }

        [When(@"I Get all commits fro the Stream ""(.*)"" from revision (.*) to revision (.*)")]
        public void WhenIGetAllCommitsFroTheStreamFromRevisionToRevision(string streamId, int minVersion, int maxVersion)
        {
            ScenarioContext.Current.Add(Engine
                .GetFrom(BucketId.ToString("N"),
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
            Generator.AddWithSameCommitSequence(quantity);
        }

        [Given(@"I Have (.*) commitAttemps with the same expected version")]
        public void GivenIHaveCommitAttempsWithTheSameExpectedVersion(int quantity)
        {
            Generator.AddWithSameStreamRevision(quantity);
        }

        [When(@"I Commit all the commit attemps")]
        public void WhenICommitAllTheCommitAttemps()
        {
            try
            {
                Generator
                    .Flush(Engine);
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
            Generator.AddInfo(count, headers);
        }

        [Given(@"I Have (.*) commit attempt")]
        public void GivenIHaveCommitAttempt(int count)
        {
            Generator.AddInfo(count);
        }


        [Then(@"The first commit should have (.*) headers")]
        public void ThenTheFirstCommitShouldHaveHeaders(int count)
        {
            FirstCommit.Headers.Count.Should().Be(count);
        }

        [Given(@"I Have the same commit attempt (.*) times")]
        public void GivenIHaveTheSameCommitAttemptTimes(int count)
        {
            CommitAttemptGenerationInfo info = Generator.CreateDefault();
            info.IncrementCommitSequence = false;
            info.IncrementStreamRevision = false;
            for (int i = 0; i < count; i++)
            {
                Generator.AddInfo(info);
            }
        }

        [Given(@"I Have (.*) snapshot")]
        public void GivenIHaveSnapshot(int count)
        {
            var snapshots = new List<Snapshot>();
            for (int i = 0; i < count; i++)
            {
                snapshots.Add(new Snapshot(BucketId.ToString("N"),
                    StreamId.ToString("N"),
                    Generator.StreamRevisionFor(StreamId), "snapshot body"));
            }
            ScenarioContext.Current.Add(snapshots.AsEnumerable());
        }

        [When(@"I Add all snapshots")]
        public void WhenIAddAllSnapshots()
        {
            foreach (Snapshot snapshot in ScenarioContext.Current.Get<IEnumerable<Snapshot>>())
            {
                Engine.AddSnapshot(snapshot);
            }
        }

        [When(@"I Ask for the snapshot for the current StreamRevision")]
        public void WhenIAskForTheSnapshotForTheCurrentStreamRevision()
        {
            ScenarioContext.Current.Add(Engine.GetSnapshot(BucketId.ToString("N"), StreamId.ToString("N"), Generator.StreamRevisionFor(StreamId)));
        }

        [Given(@"I Have snapshots for the folowing revisions")]
        public void GivenIHaveSnapshotsForTheFolowingRevisions(Table table)
        {
            var snapshots=table.Rows.Select(tr => new Snapshot(BucketId.ToString("N"),
                StreamId.ToString("N"),
                int.Parse(tr["Revision"]), "snapshot body")).ToList();
           
            ScenarioContext.Current.Add(snapshots.AsEnumerable());
        }
        [When(@"I Ask for the snapshot for the Revision (.*)")]
        public void WhenIAskForTheSnapshotForTheRevision(int revision)
        {
            ScenarioContext.Current.Add(Engine.GetSnapshot(BucketId.ToString("N"), StreamId.ToString("N"), revision));
        }

        [Then(@"the returned snapshot should be for the revision (.*)")]
        public void ThenTheReturnedSnapshotShouldBeForTheRevision(int revision)
        {
            ScenarioContext.Current.Get<ISnapshot>().StreamRevision.Should().Be(revision);
        }



        [Then(@"the returned snapshot should not be null")]
        public void ThenTheReturnedSnapshotShouldNotBeNull()
        {
            ScenarioContext.Current.Get<ISnapshot>().Should().NotBeNull();
        }
        [When(@"I Ask for the list of streams to snapshot with a threshold of (.*)")]
        public void WhenIAskForTheListOfStreamsToSnapshotWithAThresholdOf(int threshold)
        {
            ScenarioContext.Current.Add(Engine.GetStreamsToSnapshot(BucketId.ToString("N"),threshold));
        }

        [Then(@"The number of streamHeads returned should be (.*)")]
        public void ThenTheNumberOfStreamHeadsReturnedShouldBe(int count)
        {
            ScenarioContext.Current.Get<IEnumerable<IStreamHead>>().Count().Should().Be(count);
        }

        [Then(@"the streamHeads must contain the current stream Id")]
        public void ThenTheStreamHeadsMustContainTheCurrentStreamId()
        {
            ScenarioContext.Current.Get<IEnumerable<IStreamHead>>()
                .Any(sh => sh.StreamId == StreamId.ToString("N"))
                .Should()
                .BeTrue();
        }
        [When(@"I Add (.*) new commit attempts")]
        public void WhenIAddNewCommitAttempts(int count)
        {
            Generator.AddInfo(count);
        }



        #region sugar

        private EventStorePersistenceEngine Engine
        {
            get { return ScenarioContext.Current.Get<EventStorePersistenceEngine>(); }
        }

        private CommitAttemptGenerator Generator
        {
            get { return ScenarioContext.Current.Get<CommitAttemptGenerator>(); }
        }

        private Guid StreamId
        {
            get { return ScenarioContext.Current.Get<Guid>(Keys.StreamId); }
        }
        private Guid BucketId
        {
            get { return ScenarioContext.Current.Get<Guid>(Keys.BucketId); }
        }
        #endregion

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
                for (int i = 0; i < EventCount; i++)
                {
                    yield return new EventMessage
                    {
                        Body = "test" + i,
                        Headers = new Dictionary<string, object>()
                    };
                }
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

            public int StreamRevisionFor(Guid streamId)
            {

                return _attempts.Where(a => a.StreamId == streamId).Sum(a => a.EventCount)+_flushed.Where(f=>f.StreamId==streamId.ToString("N")).Sum(f=>f.Events.Count);
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

            public void Flush(EventStorePersistenceEngine engine)
            {
                foreach (CommitAttemptGenerationInfo attempt in _attempts.OrderBy(a => a.Order))
                {
                    if (attempt.IncrementStreamRevision || _streamRevision.ViewCurrent() == 0)
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