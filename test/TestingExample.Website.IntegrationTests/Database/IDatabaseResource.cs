namespace TestingExample.Website.IntegrationTests.Database;

public interface IDatabaseResource : IAsyncLifetime
{
    DatabaseConnectionString GetConnectionString();
}
