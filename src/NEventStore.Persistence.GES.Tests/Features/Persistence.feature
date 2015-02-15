Feature: Persistence

Background: 
	Given i have the following options
	| WritePageSize | ReadPageSize |
	| 50            | 50           |
	Given I have a PersistenceEngine
	Given I have initiliazed the Engine
	Given The PersistentStore is empty
	Given I Have defined a default bucket 
	Given that i have defined a default stream
	Given that i have a CommitAttemptGenerator
	
Scenario: Headers can have period in key
	Given I Have 1 commit attempt with the following headers
	| key   | value |
	| key.1 | value |
	When I Commit all the commit attemps
	And I Get all commits for the current Stream
	Then the first commit should have the following headers
	| key   | value |
	| key.1 | value |

Scenario: Successfull commits should persist the stream identifier
	Given I Have 1 commit attempt
	When I Commit all the commit attemps
	And I Get all commits for the current Stream
	Then The first commit should have the same stream identifier of the commit attempt

Scenario: Successfull commits should persist the stream revision
	Given I Have 1 commit attempt
	When I Commit all the commit attemps
	And I Get all commits for the current Stream
	Then The first commit should have the same stream revision of the commit attempt

Scenario: Successfull commits should persist the commit identifier
	Given I Have 1 commit attempt
	When I Commit all the commit attemps
	And I Get all commits for the current Stream
	Then The first commit should have the same stream identifier of the commit attempt

Scenario: Successfull commits should persist the commit sequence
	Given I Have 1 commit attempt
	When I Commit all the commit attemps
	And I Get all commits for the current Stream
	Then The first commit should have the same commit sequence of the commit attempt

Scenario: Successfull commits should persist the commit stamp
	Given I Have 1 commit attempt
	When I Commit all the commit attemps
	And I Get all commits for the current Stream
	Then The first commit should have a commit stamp within 5 seconds of the commit attemp stamp

Scenario: Successfull commits should persist all the commit attempt headers
	Given I Have 1 commit attempt with the following headers
	| key   | value  |
	| key.1 | value  |
	| key2  | value2 |
	| key3  | value3 |
	When I Commit all the commit attemps
	And I Get all commits for the current Stream
	Then The first commit should have 3 headers 

Scenario: Successfull commits should persist all the events
	Given I Have 1 commit attempt
	When I Commit all the commit attemps
	And I Get all commits for the current Stream
	Then The first commit should have the same number of events of the commit attempt

@ignore
Scenario: Successfull commits should add the commit to the set of undispatched commits
@ignore
Scenario: Successfull commits should cause the stream to be found in the list of streams to snapshot

Scenario Outline: The ICommit Enumerable should be in the correct order
	Given I Have following commit attemps that was commited in this order
	 | Order | CommitId                               | StreamId                               | EventCount |
	 | 1     | {74996A50-75BA-4995-B111-B585008C5FAE} | {1F251B9C-C872-4032-A2ED-CC848150DB9E} | 2          |
	 | 2     | {7714ED64-D769-463E-8970-35672559E217} | {1F251B9C-C872-4032-A2ED-CC848150DB9E} | 2          |
	 | 3     | {007E004A-C042-4001-977E-ACE5F7200999} | {1F251B9C-C872-4032-A2ED-CC848150DB9E} | 2          |
	 | 4     | {39DBE31A-6520-4BE7-BC91-1D9831ED4B48} | {1F251B9C-C872-4032-A2ED-CC848150DB9E} | 2          |
	When I Get all commits fro the Stream "{1F251B9C-C872-4032-A2ED-CC848150DB9E}" from revision <From> to revision <To>
	Then There should be <CommitCount> commits
	Then  The First Commit should have the CommitId "<FirstCommitId>"
	Then  The Second Commit should have the CommitId "<SeccondCommitId>"
	Examples: 
	| From | To | CommitCount | FirstCommitId                          | SeccondCommitId                        |
	| 1    | 8  | 4           | {74996A50-75BA-4995-B111-B585008C5FAE} | {7714ED64-D769-463E-8970-35672559E217} |
	| 3    | 5  | 2           | {7714ED64-D769-463E-8970-35672559E217} | {007E004A-C042-4001-977E-ACE5F7200999} |
	| 3    | 6  | 2           | {7714ED64-D769-463E-8970-35672559E217} | {007E004A-C042-4001-977E-ACE5F7200999} |
	| 7    | 7  | 1           | {39DBE31A-6520-4BE7-BC91-1D9831ED4B48} |                                        |
	
Scenario: Trying to commit the same commit attempt should raise a ConcurrencyException
	Given I Have 1 commit attempt
	Given I Have the same commit attempt 2 times
	When I Commit all the commit attemps
	Then the current Exception should be of type "NEventStore.ConcurrencyException" 
	#event store only has the concurrency exception, so we are not going to try to query the stream to check if the same commit id already exists
	#so we can't throw the DuplicateCommitException

Scenario: Trying to commit different commits with the same expected version should raise a ConcurrencyException
	Given I Have 2 commitAttemps with the same expected version
	When I Commit all the commit attemps
	Then the current Exception should be of type "NEventStore.ConcurrencyException"

Scenario: Trying to commit different commits with the same commit sequence should raise a ConcurrencyException
	Given I Have 2 commitAttemps with the same CommitSequence
	When I Commit all the commit attemps
	Then the current Exception should be of type "NEventStore.ConcurrencyException"

Scenario: Commits with more events then WritePageSize should be correctly persisted
	Given I Have following commit attemps that was commited in this order
         | Order | CommitId | StreamId | EventCount |
         | 1     |          |          | 100        |
		 | 2     |          |          | 100        |
	When I Commit all the commit attemps
	And I Get all commits for the current Stream
	Then There should be 2 commits
	And The Should be 200 Events in the commits

Scenario: Save a snapshot
	Given I Have 1 commit attempt
	Given I Have 1 snapshot
	When I Commit all the commit attemps
	And I Add all snapshots
	And I Ask for the snapshot for the current StreamRevision
	Then the returned snapshot should not be null



	



