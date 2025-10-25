using TestingExample.Website.PublishedContent;

using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace TestingExample.Website.Home;

public class HomepageRequestHandler(
    IPublishedValueFallback publishedValueFallback,
    IPublishedContentOperations publishedContentOperations)
{
    private readonly IPublishedValueFallback _publishedValueFallback = publishedValueFallback;
    private readonly IPublishedContentOperations _publishedContentOperations = publishedContentOperations;

    public ExtendedHomepage CreateHomepageViewModel(Homepage Homepage)
    {
        return new ExtendedHomepage(
            Homepage,
            _publishedValueFallback,
            [.. Homepage.Children<ContentPage>(_publishedContentOperations)
                .Select(subPage => new HomepageCard(
                    subPage.Id,
                    subPage.Name,
                    string.Empty))]);
    }
}
