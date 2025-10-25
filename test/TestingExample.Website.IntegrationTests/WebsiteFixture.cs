using Microsoft.Extensions.DependencyInjection;

using TestingExample.Website.IntegrationTests.Database;
using TestingExample.Website.IntegrationTests.Website;

using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;

namespace TestingExample.Website.IntegrationTests;

public class WebsiteFixture(IDatabaseResource database) : IAsyncLifetime
{
    private readonly IDatabaseResource _database = database;
    private readonly WebsiteResource _website = new(database);

    private AsyncServiceScope _scope;
    private HttpClient? _client = null;
    private BackofficeCredentialsProvider? _backofficeCredentialsProvider = null;

    public WebsiteResource Website => _website;

    public HttpClient Client => _client ??= _website.CreateClient();

    private BackofficeCredentialsProvider BackofficeCredentialsProvider => _backofficeCredentialsProvider ??= new BackofficeCredentialsProvider(
        _scope.ServiceProvider.GetRequiredService<IUserService>(),
        _scope.ServiceProvider.GetRequiredService<IBackOfficeUserClientCredentialsManager>()
    );

    public HttpClient CreateAnonymousClient()
    {
        var client = Website.CreateClient();
        client.BaseAddress = new Uri("https://localhost:44376");
        return client;
    }

    public async Task<HttpClient> CreateBackofficeClientAsync(CancellationToken cancellationToken = default)
    {
        var client = Website.CreateClient();
        await BackofficeCredentialsProvider.AuthenticateAsBackofficeUserAsync(client, cancellationToken);
        return client;
    }

    public async ValueTask InitializeAsync()
    {
        await _database.InitializeAsync();
        await _website.StartAsync();
        _scope = Website.Services.CreateAsyncScope();
    }

    public async ValueTask DisposeAsync()
    {
        await _scope.DisposeAsync();
        await _website.DisposeAsync();
        await _database.DisposeAsync();

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
