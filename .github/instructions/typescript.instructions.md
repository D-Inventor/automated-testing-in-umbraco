---
applyTo: 'test/TestingExample.Website.FunctionalTests/**'
---

## Project structure
All relevant typescript sources are in #file:../../test/TestingExample.Website.FunctionalTests/packages/. This folder has two sub-folders:
- scenario-builder
- functional-tests

### Scenario builder
The scenario builder is a utility that constructs a content tree in a scenario and saves the scenario in an external API.

### Functional tests
The functional tests are based on playwright and consume the scenario builder to create content scenarios for testing.

## General guidelines
- Use the typescript-testing skill when you write, modify or run tests