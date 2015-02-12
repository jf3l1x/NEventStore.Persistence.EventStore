Feature: Persistence

Background: 
	Given I have a PersistenceEngine
	Given I have initiliazed the Engine
	Given The PersistentStore is empty

@mytag
Scenario: Heades can have period in key
	Given I Commit a event with the following commit headers
	| key   | value |
	| key.1 | value |
	When I Get all commits for the current Stream
	Then the first commit should have the following headers
	| key   | value |
	| key.1 | value |
