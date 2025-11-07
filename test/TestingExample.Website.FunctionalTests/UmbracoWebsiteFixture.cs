using TestingExample.ManagementApiClient;
using TestingExample.ManagementApiClient.Authentication;
using TestingExample.ManagementApiClient.Scenario;

namespace TestingExample.Website.FunctionalTests;

[CollectionDefinition(nameof(UmbracoWebsiteFixture))]
public class UmbracoWebsiteFixture
    : ICollectionFixture<UmbracoWebsiteFixture>
    , IDisposable
{
    private bool _disposedValue;
    private readonly TokenManager _tokenManager;

    public UmbracoWebsiteFixture()
    {
        var configuration = TestConfiguration.GetConfiguration();

        BaseAddress = new Uri("https://" + configuration["domain"], UriKind.Absolute);
        HttpClient = new HttpClient(new HttpClientHandler
        {
            AllowAutoRedirect = false
        })
        {
            BaseAddress = BaseAddress
        };

        _tokenManager = new(new TokenClient(HttpClient, TestConfiguration.GetRequiredValue<TokenConfiguration>("ScenarioBuilder")));
    }

    public HttpClient HttpClient { get; }
    public Uri BaseAddress { get; }

    public ScenarioBuilder NewScenario()
    {
        var documentClient = new DocumentClient(_tokenManager, HttpClient);
        var cacheClient = new Published_CacheClient(_tokenManager, HttpClient);
        return new ScenarioBuilder(BaseAddress, documentClient, cacheClient);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                HttpClient.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
