using TestingExample.Website.PublishedContent;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace TestingExample.Website.Subpage;

public class ContentPageRequestHandler(
    IPublishedValueFallback publishedValueFallback, 
    IPublishedContentOperations publishedContentOperations)
{
    private readonly IPublishedContentOperations _publishedContentOperations = publishedContentOperations;
    private readonly IPublishedValueFallback _publishedValueFallback = publishedValueFallback;

    public ExtendedContentPage CreateSubpageViewModel(ContentPage subPage)
    {
        var parent = subPage.Parent<Homepage>(_publishedContentOperations);
        return new ExtendedContentPage(subPage, _publishedValueFallback, CreateLinkToHomepage(parent));
    }

    private LinkToHomepage? CreateLinkToHomepage(Homepage? parent)
        => parent is not null
        ? new LinkToHomepage(parent.Name, parent.Url(_publishedContentOperations)!)
        : null;
}
