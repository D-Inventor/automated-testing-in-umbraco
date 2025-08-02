using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using TestingExample.Website.IntegrationTests.Website;

namespace TestingExample.Website.IntegrationTests;

public abstract class IntegrationTestBase
    : IAsyncLifetime
{
    protected IntegrationTestBase(WebsiteResource website)
    {
        Website = website;
        IServiceScopeFactory ssf = Website.Services.GetRequiredService<IServiceScopeFactory>();
        Scope = ssf.CreateAsyncScope();
        Client = CreateHttpClient();
    }

    protected WebsiteResource Website { get; }

    protected AsyncServiceScope Scope { get; private set; }

    protected IServiceProvider ServiceProvider => Scope.ServiceProvider;

    protected HttpClient Client { get; private set; }

    public ValueTask InitializeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        await Scope.DisposeAsync();

        GC.SuppressFinalize(this);
    }

    protected static async Task<T?> DeserializeResponseAsync<T>(HttpResponseMessage response)
        where T : notnull
        => await JsonSerializer.DeserializeAsync<T>(await response.Content.ReadAsStreamAsync());

    protected T GetService<T>()
        where T : notnull
        => ServiceProvider.GetRequiredService<T>();

    protected virtual HttpClient CreateHttpClient()
    {
        return Website.CreateClient();
    }
}