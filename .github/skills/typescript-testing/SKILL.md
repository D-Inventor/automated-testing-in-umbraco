---
name: typescript-testing
description: 'Use when you need to run or write tests in typescript'
---

## Write tests in Typescript
- A test name starts with 'Should' and describes the expected behaviour in at most 7 words
- Do NOT implement the feature, write just enough so that the test can run
- After writing a test, run the test
- At best, the test fails on the assertion at the end
- At second best, the test fails on an error in the system under test

## Run tests in Typescript
Tests in typescript should be executed using #tool:execute/runTests .
Avoid console commands.