---
name: typescript-testing
description: >
  Use this for any typescript testing task: writing, adding, editing, running or executing
  unit or integration tests
user-invocable: false
---

## Constraints
- Do NOT implement the feature
- Avoid console commands

## Workflow for writing tests
1. Read other tests to understand the conventions
2. Search for relevant source code to understand how to write the test
2. Write the test with #tool:edit
3. Run the test to make sure that it executes

## How to write tests in Typescript
- Write just enough so that the test can run
- At best, the test fails on the assertion at the end
- At second best, the test fails on an error in the system under test

## How to run tests in Typescript
- Run the test using #tool:execute/runTests