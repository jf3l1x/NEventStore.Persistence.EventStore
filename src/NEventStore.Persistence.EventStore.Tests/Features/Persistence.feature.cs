﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:1.9.0.77
//      SpecFlow Generator Version:1.9.0.0
//      Runtime Version:4.0.30319.34209
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace NEventStore.Persistence.EventStore.Tests.Features
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.9.0.77")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Persistence")]
    public partial class PersistenceFeature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "Persistence.feature"
#line hidden
        
        [NUnit.Framework.TestFixtureSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Persistence", "", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.TestFixtureTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public virtual void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioSetup(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioStart(scenarioInfo);
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        public virtual void FeatureBackground()
        {
#line 3
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "WritePageSize",
                        "ReadPageSize",
                        "MinimunSnapshotThreshold"});
            table1.AddRow(new string[] {
                        "50",
                        "50",
                        "1"});
#line 4
 testRunner.Given("i have the following options", ((string)(null)), table1, "Given ");
#line 7
 testRunner.Given("I have a PersistenceEngine", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 8
 testRunner.Given("I have initiliazed the Engine", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 9
 testRunner.Given("The PersistentStore is empty", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 10
 testRunner.Given("I Have defined a default bucket", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 11
 testRunner.Given("that i have defined a default stream", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 12
 testRunner.Given("that i have a CommitAttemptGenerator", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Headers can have period in key")]
        public virtual void HeadersCanHavePeriodInKey()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Headers can have period in key", ((string[])(null)));
#line 14
this.ScenarioSetup(scenarioInfo);
#line 3
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                        "key",
                        "value"});
            table2.AddRow(new string[] {
                        "key.1",
                        "value"});
#line 15
 testRunner.Given("I Have 1 commit attempt with the following headers", ((string)(null)), table2, "Given ");
#line 18
 testRunner.When("I Commit all the commit attemps", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 19
 testRunner.And("I Get all commits for the current Stream", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            TechTalk.SpecFlow.Table table3 = new TechTalk.SpecFlow.Table(new string[] {
                        "key",
                        "value"});
            table3.AddRow(new string[] {
                        "key.1",
                        "value"});
#line 20
 testRunner.Then("the first commit should have the following headers", ((string)(null)), table3, "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Successfull commits should persist the stream identifier")]
        public virtual void SuccessfullCommitsShouldPersistTheStreamIdentifier()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Successfull commits should persist the stream identifier", ((string[])(null)));
#line 24
this.ScenarioSetup(scenarioInfo);
#line 3
this.FeatureBackground();
#line 25
 testRunner.Given("I Have 1 commit attempt", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 26
 testRunner.When("I Commit all the commit attemps", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 27
 testRunner.And("I Get all commits for the current Stream", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 28
 testRunner.Then("The first commit should have the same stream identifier of the commit attempt", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Successfull commits should persist the stream revision")]
        public virtual void SuccessfullCommitsShouldPersistTheStreamRevision()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Successfull commits should persist the stream revision", ((string[])(null)));
#line 30
this.ScenarioSetup(scenarioInfo);
#line 3
this.FeatureBackground();
#line 31
 testRunner.Given("I Have 1 commit attempt", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 32
 testRunner.When("I Commit all the commit attemps", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 33
 testRunner.And("I Get all commits for the current Stream", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 34
 testRunner.Then("The first commit should have the same stream revision of the commit attempt", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Successfull commits should persist the commit identifier")]
        public virtual void SuccessfullCommitsShouldPersistTheCommitIdentifier()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Successfull commits should persist the commit identifier", ((string[])(null)));
#line 36
this.ScenarioSetup(scenarioInfo);
#line 3
this.FeatureBackground();
#line 37
 testRunner.Given("I Have 1 commit attempt", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 38
 testRunner.When("I Commit all the commit attemps", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 39
 testRunner.And("I Get all commits for the current Stream", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 40
 testRunner.Then("The first commit should have the same stream identifier of the commit attempt", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Successfull commits should persist the commit sequence")]
        public virtual void SuccessfullCommitsShouldPersistTheCommitSequence()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Successfull commits should persist the commit sequence", ((string[])(null)));
#line 42
this.ScenarioSetup(scenarioInfo);
#line 3
this.FeatureBackground();
#line 43
 testRunner.Given("I Have 1 commit attempt", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 44
 testRunner.When("I Commit all the commit attemps", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 45
 testRunner.And("I Get all commits for the current Stream", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 46
 testRunner.Then("The first commit should have the same commit sequence of the commit attempt", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Successfull commits should persist the commit stamp")]
        public virtual void SuccessfullCommitsShouldPersistTheCommitStamp()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Successfull commits should persist the commit stamp", ((string[])(null)));
#line 48
this.ScenarioSetup(scenarioInfo);
#line 3
this.FeatureBackground();
#line 49
 testRunner.Given("I Have 1 commit attempt", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 50
 testRunner.When("I Commit all the commit attemps", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 51
 testRunner.And("I Get all commits for the current Stream", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 52
 testRunner.Then("The first commit should have a commit stamp within 5 seconds of the commit attemp" +
                    " stamp", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Successfull commits should persist all the commit attempt headers")]
        public virtual void SuccessfullCommitsShouldPersistAllTheCommitAttemptHeaders()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Successfull commits should persist all the commit attempt headers", ((string[])(null)));
#line 54
this.ScenarioSetup(scenarioInfo);
#line 3
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table4 = new TechTalk.SpecFlow.Table(new string[] {
                        "key",
                        "value"});
            table4.AddRow(new string[] {
                        "key.1",
                        "value"});
            table4.AddRow(new string[] {
                        "key2",
                        "value2"});
            table4.AddRow(new string[] {
                        "key3",
                        "value3"});
#line 55
 testRunner.Given("I Have 1 commit attempt with the following headers", ((string)(null)), table4, "Given ");
#line 60
 testRunner.When("I Commit all the commit attemps", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 61
 testRunner.And("I Get all commits for the current Stream", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 62
 testRunner.Then("The first commit should have 3 headers", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Successfull commits should persist all the events")]
        public virtual void SuccessfullCommitsShouldPersistAllTheEvents()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Successfull commits should persist all the events", ((string[])(null)));
#line 64
this.ScenarioSetup(scenarioInfo);
#line 3
this.FeatureBackground();
#line 65
 testRunner.Given("I Have 1 commit attempt", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 66
 testRunner.When("I Commit all the commit attemps", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 67
 testRunner.And("I Get all commits for the current Stream", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 68
 testRunner.Then("The first commit should have the same number of events of the commit attempt", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Successfull commits should add the commit to the set of undispatched commits")]
        [NUnit.Framework.IgnoreAttribute()]
        public virtual void SuccessfullCommitsShouldAddTheCommitToTheSetOfUndispatchedCommits()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Successfull commits should add the commit to the set of undispatched commits", new string[] {
                        "ignore"});
#line 71
this.ScenarioSetup(scenarioInfo);
#line 3
this.FeatureBackground();
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Successfull commits should cause the stream to be found in the list of streams to" +
            " snapshot")]
        [NUnit.Framework.IgnoreAttribute()]
        public virtual void SuccessfullCommitsShouldCauseTheStreamToBeFoundInTheListOfStreamsToSnapshot()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Successfull commits should cause the stream to be found in the list of streams to" +
                    " snapshot", new string[] {
                        "ignore"});
#line 73
this.ScenarioSetup(scenarioInfo);
#line 3
this.FeatureBackground();
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("The ICommit Enumerable should be in the correct order")]
        [NUnit.Framework.TestCaseAttribute("1", "8", "4", "{74996A50-75BA-4995-B111-B585008C5FAE}", "{7714ED64-D769-463E-8970-35672559E217}", null)]
        [NUnit.Framework.TestCaseAttribute("3", "5", "2", "{7714ED64-D769-463E-8970-35672559E217}", "{007E004A-C042-4001-977E-ACE5F7200999}", null)]
        [NUnit.Framework.TestCaseAttribute("3", "6", "2", "{7714ED64-D769-463E-8970-35672559E217}", "{007E004A-C042-4001-977E-ACE5F7200999}", null)]
        [NUnit.Framework.TestCaseAttribute("7", "7", "1", "{39DBE31A-6520-4BE7-BC91-1D9831ED4B48}", "", null)]
        public virtual void TheICommitEnumerableShouldBeInTheCorrectOrder(string from, string to, string commitCount, string firstCommitId, string seccondCommitId, string[] exampleTags)
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("The ICommit Enumerable should be in the correct order", exampleTags);
#line 75
this.ScenarioSetup(scenarioInfo);
#line 3
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table5 = new TechTalk.SpecFlow.Table(new string[] {
                        "Order",
                        "CommitId",
                        "StreamId",
                        "EventCount"});
            table5.AddRow(new string[] {
                        "1",
                        "{74996A50-75BA-4995-B111-B585008C5FAE}",
                        "{1F251B9C-C872-4032-A2ED-CC848150DB9E}",
                        "2"});
            table5.AddRow(new string[] {
                        "2",
                        "{7714ED64-D769-463E-8970-35672559E217}",
                        "{1F251B9C-C872-4032-A2ED-CC848150DB9E}",
                        "2"});
            table5.AddRow(new string[] {
                        "3",
                        "{007E004A-C042-4001-977E-ACE5F7200999}",
                        "{1F251B9C-C872-4032-A2ED-CC848150DB9E}",
                        "2"});
            table5.AddRow(new string[] {
                        "4",
                        "{39DBE31A-6520-4BE7-BC91-1D9831ED4B48}",
                        "{1F251B9C-C872-4032-A2ED-CC848150DB9E}",
                        "2"});
#line 76
 testRunner.Given("I Have following commit attemps that was commited in this order", ((string)(null)), table5, "Given ");
#line 82
 testRunner.When(string.Format("I Get all commits fro the Stream \"{{1F251B9C-C872-4032-A2ED-CC848150DB9E}}\" from " +
                        "revision {0} to revision {1}", from, to), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 83
 testRunner.Then(string.Format("There should be {0} commits", commitCount), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 84
 testRunner.Then(string.Format("The First Commit should have the CommitId \"{0}\"", firstCommitId), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 85
 testRunner.Then(string.Format("The Second Commit should have the CommitId \"{0}\"", seccondCommitId), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Trying to commit the same commit attempt should raise a ConcurrencyException")]
        public virtual void TryingToCommitTheSameCommitAttemptShouldRaiseAConcurrencyException()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Trying to commit the same commit attempt should raise a ConcurrencyException", ((string[])(null)));
#line 93
this.ScenarioSetup(scenarioInfo);
#line 3
this.FeatureBackground();
#line 94
 testRunner.Given("I Have 1 commit attempt", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 95
 testRunner.Given("I Have the same commit attempt 2 times", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 96
 testRunner.When("I Commit all the commit attemps", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 97
 testRunner.Then("the current Exception should be of type \"NEventStore.ConcurrencyException\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Trying to commit different commits with the same expected version should raise a " +
            "ConcurrencyException")]
        public virtual void TryingToCommitDifferentCommitsWithTheSameExpectedVersionShouldRaiseAConcurrencyException()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Trying to commit different commits with the same expected version should raise a " +
                    "ConcurrencyException", ((string[])(null)));
#line 101
this.ScenarioSetup(scenarioInfo);
#line 3
this.FeatureBackground();
#line 102
 testRunner.Given("I Have 2 commitAttemps with the same expected version", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 103
 testRunner.When("I Commit all the commit attemps", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 104
 testRunner.Then("the current Exception should be of type \"NEventStore.ConcurrencyException\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Trying to commit different commits with the same commit sequence should raise a C" +
            "oncurrencyException")]
        public virtual void TryingToCommitDifferentCommitsWithTheSameCommitSequenceShouldRaiseAConcurrencyException()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Trying to commit different commits with the same commit sequence should raise a C" +
                    "oncurrencyException", ((string[])(null)));
#line 106
this.ScenarioSetup(scenarioInfo);
#line 3
this.FeatureBackground();
#line 107
 testRunner.Given("I Have 2 commitAttemps with the same CommitSequence", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 108
 testRunner.When("I Commit all the commit attemps", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 109
 testRunner.Then("the current Exception should be of type \"NEventStore.ConcurrencyException\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Commits with more events then WritePageSize should be correctly persisted")]
        public virtual void CommitsWithMoreEventsThenWritePageSizeShouldBeCorrectlyPersisted()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Commits with more events then WritePageSize should be correctly persisted", ((string[])(null)));
#line 111
this.ScenarioSetup(scenarioInfo);
#line 3
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table6 = new TechTalk.SpecFlow.Table(new string[] {
                        "Order",
                        "CommitId",
                        "StreamId",
                        "EventCount"});
            table6.AddRow(new string[] {
                        "1",
                        "",
                        "",
                        "100"});
            table6.AddRow(new string[] {
                        "2",
                        "",
                        "",
                        "100"});
#line 112
 testRunner.Given("I Have following commit attemps that was commited in this order", ((string)(null)), table6, "Given ");
#line 116
 testRunner.When("I Commit all the commit attemps", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 117
 testRunner.And("I Get all commits for the current Stream", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 118
 testRunner.Then("There should be 2 commits", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 119
 testRunner.And("The Should be 200 Events in the commits", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Save a snapshot")]
        public virtual void SaveASnapshot()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Save a snapshot", ((string[])(null)));
#line 121
this.ScenarioSetup(scenarioInfo);
#line 3
this.FeatureBackground();
#line 122
 testRunner.Given("I Have 1 commit attempt", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 123
 testRunner.Given("I Have 1 snapshot", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 124
 testRunner.When("I Commit all the commit attemps", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 125
 testRunner.And("I Add all snapshots", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 126
 testRunner.And("I Ask for the snapshot for the current StreamRevision", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 127
 testRunner.Then("the returned snapshot should not be null", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Retrieving a snapshot")]
        public virtual void RetrievingASnapshot()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Retrieving a snapshot", ((string[])(null)));
#line 129
this.ScenarioSetup(scenarioInfo);
#line 3
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table7 = new TechTalk.SpecFlow.Table(new string[] {
                        "Order",
                        "CommitId",
                        "StreamId",
                        "EventCount"});
            table7.AddRow(new string[] {
                        "1",
                        "",
                        "",
                        "2"});
            table7.AddRow(new string[] {
                        "2",
                        "",
                        "",
                        "2"});
            table7.AddRow(new string[] {
                        "3",
                        "",
                        "",
                        "2"});
#line 130
 testRunner.Given("I Have following commit attemps that was commited in this order", ((string)(null)), table7, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table8 = new TechTalk.SpecFlow.Table(new string[] {
                        "Revision"});
            table8.AddRow(new string[] {
                        "1"});
            table8.AddRow(new string[] {
                        "3"});
            table8.AddRow(new string[] {
                        "5"});
#line 135
 testRunner.Given("I Have snapshots for the folowing revisions", ((string)(null)), table8, "Given ");
#line 140
 testRunner.When("I Commit all the commit attemps", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 141
 testRunner.And("I Add all snapshots", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 142
 testRunner.And("I Ask for the snapshot for the Revision 4", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 143
 testRunner.Then("the returned snapshot should not be null", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 144
 testRunner.And("the returned snapshot should be for the revision 3", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Retrieving a list of streams to snapshot should consider if there\'s more then the" +
            " MinimunSnapshotThreshold")]
        public virtual void RetrievingAListOfStreamsToSnapshotShouldConsiderIfThereSMoreThenTheMinimunSnapshotThreshold()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Retrieving a list of streams to snapshot should consider if there\'s more then the" +
                    " MinimunSnapshotThreshold", ((string[])(null)));
#line 146
this.ScenarioSetup(scenarioInfo);
#line 3
this.FeatureBackground();
#line 147
 testRunner.Given("I Have 2 commit attempt", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 148
 testRunner.When("I Commit all the commit attemps", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 149
 testRunner.And("I Ask for the list of streams to snapshot with a threshold of 1", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 150
 testRunner.Then("The number of streamHeads returned should be 1", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 151
 testRunner.And("the streamHeads mus contain the current stream Id", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Retrieving a list of streams to snapshot should not consider if the number of eve" +
            "nts is equals to the MinimunSnapshotThreshold")]
        public virtual void RetrievingAListOfStreamsToSnapshotShouldNotConsiderIfTheNumberOfEventsIsEqualsToTheMinimunSnapshotThreshold()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Retrieving a list of streams to snapshot should not consider if the number of eve" +
                    "nts is equals to the MinimunSnapshotThreshold", ((string[])(null)));
#line 153
this.ScenarioSetup(scenarioInfo);
#line 3
this.FeatureBackground();
#line 154
 testRunner.Given("I Have 1 commit attempt", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 155
 testRunner.When("I Commit all the commit attemps", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 156
 testRunner.And("I Ask for the list of streams to snapshot with a threshold of 1", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 157
 testRunner.Then("The number of streamHeads returned should be 0", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("When a snapshot has been added to the most recent commit of a stream")]
        public virtual void WhenASnapshotHasBeenAddedToTheMostRecentCommitOfAStream()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("When a snapshot has been added to the most recent commit of a stream", ((string[])(null)));
#line 159
this.ScenarioSetup(scenarioInfo);
#line 3
this.FeatureBackground();
#line 160
 testRunner.Given("I Have 2 commit attempt", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 161
 testRunner.And("I Have 1 snapshot", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 162
 testRunner.When("I Commit all the commit attemps", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 163
 testRunner.And("I Add all snapshots", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 164
 testRunner.And("I Ask for the list of streams to snapshot with a threshold of 1", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 165
 testRunner.Then("The number of streamHeads returned should be 0", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
