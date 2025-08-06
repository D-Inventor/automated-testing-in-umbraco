using System.Net;

namespace TestingExample.Website.IntegrationTests;

[Collection(nameof(SqlServerWebsiteFixture))]
public sealed class BackofficeApiTests(SqlServerWebsiteFixture websiteFixture)
{
    private readonly SqlServerWebsiteFixture _websiteFixture = websiteFixture;

    [Fact]
    public async Task ShouldReturnOkResult()
    {
        // given
        var backofficeClient = await _websiteFixture.CreateBackofficeClientAsync(TestContext.Current.CancellationToken);

        // when
        var response = await backofficeClient.GetAsync("/umbraco/management/api/v1/exampleapi", TestContext.Current.CancellationToken);

        // then
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}