
using Microsoft.Playwright;

using TestingExample.ManagementApiClient.Scenario.Model;
using TestingExample.Website.FunctionalTests.Domain;

namespace TestingExample.Website.FunctionalTests.PageObjects;

public sealed class HomePageObject(IPage page, PageModel<WebsiteContent> pageModel)
{
    private readonly IPage _page = page;
    private readonly PageModel<WebsiteContent> _pageModel = pageModel;

    internal Task GoToAsync()
    {
        return _page.GotoAsync(_pageModel.Url(Locale.Culture("en-US")).ToString());
    }

    internal ILocator Title => _page.Locator("h1");
}
