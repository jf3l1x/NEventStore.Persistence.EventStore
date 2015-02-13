Feature: Persistence

Background: 
	Given I have a PersistenceEngine
	Given I have initiliazed the Engine
	Given The PersistentStore is empty
	Given I Have a commit attempt

Scenario: Heades can have period in key
	
	Given I set the following headers in the commit attempt
	| key   | value |
	| key.1 | value |
	When I Commit the commitAttempt
	And I Get all commits for the current Stream
	Then the first commit should have the following headers
	| key   | value |
	| key.1 | value |

Scenario: Successfull commits should persist the stream identifier
	When I Commit the commitAttempt
	And I Get all commits for the current Stream
	Then The first commit should have the same stream identifier of the commit attempt

Scenario: Successfull commits should persist the stream revision
	When I Commit the commitAttempt
	And I Get all commits for the current Stream
	Then The first commit should have the same stream revision of the commit attempt

Scenario: Successfull commits should persist the commit identifier
	When I Commit the commitAttempt
	And I Get all commits for the current Stream
	Then The first commit should have the same stream identifier of the commit attempt

Scenario: Successfull commits should persist the commit sequence
	When I Commit the commitAttempt
	And I Get all commits for the current Stream
	Then The first commit should have the same commit sequence of the commit attempt

Scenario: Successfull commits should persist the commit stamp
	When I Commit the commitAttempt
	And I Get all commits for the current Stream
	Then The first commit should have a commit stamp within 5 seconds of the commit attemp stamp

Scenario: Successfull commits should persist all the commit attempt headers
	Given I set the following headers in the commit attempt
	| key   | value  |
	| key.1 | value  |
	| key2  | value2 |
	| key3  | value3 |
	When I Commit the commitAttempt
	And I Get all commits for the current Stream
	Then The first commit should have the same number of headers of the commit attempt

Scenario: Successfull commits should persist all the events
	When I Commit the commitAttempt
	And I Get all commits for the current Stream
	Then The first commit should have the same number of events of the commit attempt

@ignore
Scenario: Successfull commits should add the commit to the set of undispatched commits
@ignore
Scenario: Successfull commits should cause the stream to be found in the list of streams to snapshot

Scenario: The ICommit Enumerable should be in the correct order
	Given I Have following commit attemps that was commited in this order
	 | Order | CommitId                               | StreamId                               | EventCount |
	 | 1     | {74996A50-75BA-4995-B111-B585008C5FAE} | {1F251B9C-C872-4032-A2ED-CC848150DB9E} | 2          |
	 | 2     | {7714ED64-D769-463E-8970-35672559E217} | {1F251B9C-C872-4032-A2ED-CC848150DB9E} | 2          |
	 | 3     | {007E004A-C042-4001-977E-ACE5F7200999} | {1F251B9C-C872-4032-A2ED-CC848150DB9E} | 2          |
	 | 4     | {39DBE31A-6520-4BE7-BC91-1D9831ED4B48} | {1F251B9C-C872-4032-A2ED-CC848150DB9E} | 2          |
	When I Get all commits fro the Stream "{1F251B9C-C872-4032-A2ED-CC848150DB9E}" from revision 3 to revision 5
	Then There should be 2 commits
	Then  The First Commit should have the CommitId "{7714ED64-D769-463E-8970-35672559E217}"
	Then  The Second Commit should have the CommitId "{007E004A-C042-4001-977E-ACE5F7200999}"

	



