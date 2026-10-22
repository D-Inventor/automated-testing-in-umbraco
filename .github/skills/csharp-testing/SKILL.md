---
name: csharp-testing
description: 'Use when you need to run tests written in C#'
---

# C# Testing workflow
Follow these steps:
1. build the test project using the `dotnet build` command
2. Execute the tests using #tool:execute/runTests .

## Avoid this
- Avoid building the entire solution.
- Avoid running tests using the `dotnet test` command
