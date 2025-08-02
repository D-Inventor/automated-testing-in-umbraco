using System.Net;

namespace TestingExample.Website.IntegrationTests;

[Collection(nameof(SqlServerWebsiteFixture))]
public class HomepageTests(SqlServerWebsiteFixture websiteFixture)
    : IntegrationTestBase(websiteFixture.Website)
{
    [Fact]
    public async Task ShouldGetTheHomepage()
    {
        // given / when
        var response = await Client.GetAsync("/", TestContext.Current.CancellationToken);

        // then
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}