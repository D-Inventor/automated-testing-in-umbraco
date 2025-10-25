using Microsoft.Playwright.Xunit.v3;

using TestingExample.ManagementApiClient.Scenario;
using TestingExample.ManagementApiClient.Scenario.Model;
using TestingExample.Website.FunctionalTests.PageObjects;

namespace TestingExample.Website.FunctionalTests;

[Collection(nameof(UmbracoWebsiteFixture))]
public sealed class HomepageTests(UmbracoWebsiteFixture website)
    : PageTest
{
    public UmbracoWebsiteFixture Website { get; } = website;
    public ScenarioBuilder Scenario { get; } = website.NewScenario().WithBasicContent();

    [Fact]
    public async Task ShouldDisplayTitleFromContent()
    {
        // given
        Scenario.Homepage()
            .HasContent(Variation.Invariant, content => content.WithHeader(title: "Welcome to the website"));
        await Scenario.BuildAsync(TestContext.Current.CancellationToken);
        var homepage = new HomePageObject(await Browser.NewPageAsync(), Scenario.Website());

        // when
        await homepage.GoToAsync();

        // then
        await Expect(homepage.Title).ToHaveTextAsync("Welcome to the website");
    }
}
