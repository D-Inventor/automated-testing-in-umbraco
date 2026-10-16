---
applyTo: '**/*Tests.cs'
---

- Unit tests are written with xUnit 3
- Unit tests follow the naming convention for test driven development using given, when, then.
- The words "given", "when" and "then" are always included as comments in the test method. The code for "given" and "when" may be combined if it fits on a single line, but "then" should always be separate.
- The test method names should be descriptive and begin with the word "Should" to indicate the expected behavior. For example, "ShouldReturnTrueWhenConditionIsMet".
- The variable upon which is asserted should be named "result" to indicate the value being tested.