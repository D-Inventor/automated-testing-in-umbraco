using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace TestingExample.Website.Subpage;

[HideFromTypeFinder]
public class ExtendedExampleSubpage(
    ExampleSubpage content,
    IPublishedValueFallback publishedValueFallback,
    LinkToHomepage? parentLink)
    : ExampleSubpage(content, publishedValueFallback)
{
    public LinkToHomepage? ParentLink { get; } = parentLink;
}

public record LinkToHomepage(string Title, Uri Url);