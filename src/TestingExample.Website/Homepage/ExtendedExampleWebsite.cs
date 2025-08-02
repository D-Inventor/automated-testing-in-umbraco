using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace TestingExample.Website.Homepage;

[HideFromTypeFinder]
public class ExtendedExampleWebsite(
    ExampleWebsite content,
    IPublishedValueFallback publishedValueFallback,
    List<ExampleCard> cards)
    : ExampleWebsite(content, publishedValueFallback)
{
    public List<ExampleCard> Cards { get; } = cards;
}

public record ExampleCard(int Id, string Title, string Introduction);