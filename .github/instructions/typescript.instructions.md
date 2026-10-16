---
applyTo: 'test/TestingExample.Website.FunctionalTests/**'
---

## Project structure
All typescript sources are in #file:../../test/TestingExample.Website.FunctionalTests/.
This folder is the root folder for all typescript operations and commands.
This folder has two packages:
- packages/scenario-builder
- packages/functional-tests

### Scenario builder
The scenario builder is a utility that constructs a content tree in a scenario and saves the scenario in an external API.

### Functional tests
The functional tests are based on playwright and consume the scenario builder to create content scenarios for testing.

## General guidelines
- This project uses PNPM