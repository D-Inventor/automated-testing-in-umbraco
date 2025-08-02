# Umbraco 16 automated testing setup

This project is a fully functioning setup for automated testing with Umbraco 16. You can use this project as a reference or starting point to get started with testing on your Umbraco website. The tests are set up with Test Driven Development (TDD) in mind.

## Tools

The most important tools that are used in the automated tests are as follows:

| Name                                           | Description                                                                                                                                                                                                                                                                 |
| ---------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| [xUnit v3](https://xunit.net/?tabs=cs)         | The testing framework. You can use any testing framework that you like though                                                                                                                                                                                               |
| [NSubstitute](https://nsubstitute.github.io/)  | Library for mocking. Any mocking library will work. This project doesn't do extensive mocking, but for example `IPublishedValueFallback` is a mandatory parameter for any published content item, even if you don't actually use it. It's just convenient to insert a mock. |
| [Test Containers](https://testcontainers.com/) | Automatically creates docker containers while running tests. It is used to create an empty SQL Server database that is automatically cleaned up after testing. It is required to have **Docker Desktop** installed and running while running these tests.                   |

For a more exhaustive list of tools, I recommend checking out the .csproj files in each testing project.

## How to use this project

Use this project as a reference to understand how to get started with automated testing in an Umbraco website. Each testing project contains its own readme that explains what things it demonstrates and which files you should check out.

- [Unit testing project](./test/TestingExample.Website.UnitTests/)
- [Integration testing project](./test/TestingExample.Website.IntegrationTests/)

## Running this project locally

Running the tests on your own computer is a good way to get familiar with automated testing. Make sure that you install [Docker](https://www.docker.com/) on your PC and ensure that Docker is running. Clone this repository and run the following command to run the tests:

```
dotnet test
```

Alternatively, you can use your automated test explorer in your IDE.
