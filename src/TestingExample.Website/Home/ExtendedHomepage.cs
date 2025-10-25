using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace TestingExample.Website.Home;

[HideFromTypeFinder]
public class ExtendedHomepage(
    Umbraco.Cms.Web.Common.PublishedModels.Homepage content,
    IPublishedValueFallback publishedValueFallback,
    List<HomepageCard> cards)
    : Umbraco.Cms.Web.Common.PublishedModels.Homepage(content, publishedValueFallback)
{
    public List<HomepageCard> Cards { get; } = cards;
}

public record HomepageCard(int Id, string Title, string Introduction);