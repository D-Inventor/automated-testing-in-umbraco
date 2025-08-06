using System.Net;

namespace TestingExample.Website.IntegrationTests;

[Collection(nameof(SqlServerWebsiteFixture))]
public sealed class HomepageTests(SqlServerWebsiteFixture websiteFixture)
{
    private readonly SqlServerWebsiteFixture _websiteFixture = websiteFixture;

    [Fact]
    public async Task ShouldGetTheHomepage()
    {
        // given
        var client = _websiteFixture.CreateAnonymousClient();

        // when
        var response = await client.GetAsync("/", TestContext.Current.CancellationToken);

        // then
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}