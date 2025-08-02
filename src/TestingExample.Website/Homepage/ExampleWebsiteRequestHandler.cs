using TestingExample.Website.PublishedContent;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace TestingExample.Website.Homepage;

public class ExampleWebsiteRequestHandler(
    IPublishedValueFallback publishedValueFallback,
    IPublishedContentOperations publishedContentOperations)
{
    private readonly IPublishedValueFallback _publishedValueFallback = publishedValueFallback;
    private readonly IPublishedContentOperations _publishedContentOperations = publishedContentOperations;

    public ExtendedExampleWebsite CreateHomepageViewModel(ExampleWebsite exampleWebsite)
    {
        return new ExtendedExampleWebsite(
            exampleWebsite,
            _publishedValueFallback,
            [.. exampleWebsite.Children<ExampleSubpage>(_publishedContentOperations)
                .Select(subPage => new ExampleCard(
                    subPage.Id,
                    subPage.Name,
                    subPage.Introduction ?? string.Empty))]);
    }
}
