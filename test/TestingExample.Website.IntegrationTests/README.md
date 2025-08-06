# Integration testing in Umbraco

This testing project demonstrates integration testing with Umbraco content. The test starts the Umbraco site in-memory using WebApplicationFactory and verifies that requests to the site return expected results.

## About running an Umbraco site in-memory

To start the website in-memory, this project creates two resources: A database resource and a website resource. A collection fixture combines these together into a working in-memory website. This separation allows to choose which database implementation to test against. You can use Sqlite or SQL Server for example.

The website resource modifies some configurations and replaces some Umbraco services to ensure consistent results while running tests. Additionally, the website resource imports all content and settings with uSync so that the tests start in a consistent state with expected content. The database resource creates an empty database and ensures that it is cleaned up after all tests have run.

### Relevant source code

- [Implementation of the SQL Server database using Test Containers](./Database/SqlServerDatabase.cs)
- [Implementation of the website resource](./Website/WebsiteResource.cs)
- [Test-specific configurations](./integration.settings.json)
- [Example integration test](./HomepageTests.cs)

### work-arounds

Specifically for xUnit v3, a compromise was necessary. In short: type finder settings must be configured to prevent Umbraco from attempting to load test-assemblies. This configuration must be added to [appsettings.Development.json](../../src/TestingExample.Website/appsettings.Development.json) in the source code. It does not work in integration.settings.json:

```json
{
  "Umbraco": {
    "CMS": {
      "TypeFinder": {
        "AdditionalAssemblyExclusionEntries": [
          "xunit.abstractions",
          "xunit.runner.visualstudio.testadapter",
          "Mono.Cecil",
          "xunit.v3.runner.utility.netcore"
        ],
        "AssembliesAcceptingLoadExceptions": "*"
      }
    }
  }
}
```

## Running tests on the backoffice

The management api is protected, so you can't simply call an endpoint and get a response. You need an authenticated backoffice user to fetch a result from a backoffice api endpoint.

### Relevant source code
- [Example test that tests a backoffice endpoint](./BackofficeApiTests.cs)
- [The BackofficeCredentialsProvider authenticates an HttpClient so that it can acces backoffice APIs](./Website/BackofficeCredentialsProvider.cs)