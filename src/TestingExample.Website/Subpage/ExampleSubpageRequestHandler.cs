using TestingExample.Website.PublishedContent;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace TestingExample.Website.Subpage;

public class ExampleSubpageRequestHandler(
    IPublishedValueFallback publishedValueFallback, 
    IPublishedContentOperations publishedContentOperations)
{
    private readonly IPublishedContentOperations _publishedContentOperations = publishedContentOperations;
    private readonly IPublishedValueFallback _publishedValueFallback = publishedValueFallback;

    public ExtendedExampleSubpage CreateSubpageViewModel(ExampleSubpage subPage)
    {
        var parent = subPage.Parent<ExampleWebsite>(_publishedContentOperations);
        return new ExtendedExampleSubpage(subPage, _publishedValueFallback, CreateLinkToHomepage(parent));
    }

    private LinkToHomepage? CreateLinkToHomepage(ExampleWebsite? parent)
        => parent is not null
        ? new LinkToHomepage(parent.Name, parent.Url(_publishedContentOperations)!)
        : null;
}
