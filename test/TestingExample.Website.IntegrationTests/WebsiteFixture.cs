using TestingExample.Website.IntegrationTests.Database;
using TestingExample.Website.IntegrationTests.Website;

namespace TestingExample.Website.IntegrationTests;

public class WebsiteFixture(IDatabaseResource database)
    : IAsyncLifetime
{
    private readonly IDatabaseResource _database = database;
    private readonly WebsiteResource _website = new(database);

    public WebsiteResource Website => _website;

    public async ValueTask InitializeAsync()
    {
        await _database.InitializeAsync();
        await Website.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_website is not null) await _website.DisposeAsync();
        if (_database is not null) await _database.DisposeAsync();

        GC.SuppressFinalize(this);
    }
}

[CollectionDefinition(nameof(SqliteWebsiteFixture))]
public sealed class SqliteWebsiteFixture()
    : WebsiteFixture(new SqliteDatabase())
    , ICollectionFixture<SqliteWebsiteFixture>
{
}

[CollectionDefinition(nameof(SqlServerWebsiteFixture))]
public sealed class SqlServerWebsiteFixture()
    : WebsiteFixture(new SqlServerDatabase())
    , ICollectionFixture<SqlServerWebsiteFixture>
{
}