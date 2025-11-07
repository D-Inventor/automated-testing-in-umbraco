namespace TestingExample.ManagementApiClient.UnitTests.TestDoubles;

public class CacheClientStub : ICacheClient
{
    public Task PostPublishedCacheReloadAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}