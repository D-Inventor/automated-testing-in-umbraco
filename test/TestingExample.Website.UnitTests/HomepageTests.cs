using NSubstitute;

using TestingExample.Website.Home;
using TestingExample.Website.UnitTests.PublishedContent;

using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace TestingExample.Website.UnitTests;

public class HomepageTests
{
    private readonly Homepage _homepage;
    private readonly FakePublishedContentOperations _publishedContentOperations = new ();
    private readonly HomepageRequestHandler _requestHandler;

    public HomepageTests()
    {
        _homepage = FakePublishedContent.Generate<Homepage>();
        _requestHandler = new(Substitute.For<IPublishedValueFallback>(), _publishedContentOperations);
    }

    [Fact]
    public void ShouldCreateHomepageViewModel()
    {
        // given / when
        var result = _requestHandler.CreateHomepageViewModel(_homepage);

        // then
        Assert.NotNull(result);
    }

    [Fact]
    public void ShouldCreateCardsForSelectedPages()
    {
        // given
        var detailpage1 = FakePublishedContent.Generate<ContentPage>();
        var detailpage2 = FakePublishedContent.Generate<ContentPage>();
        _homepage.PropertyValues()
            .Set(item => item.FeaturedContent, [detailpage1, detailpage2]);

        _publishedContentOperations.SetChildren(_homepage, [detailpage1, detailpage2]);

        // when
        var result = _requestHandler.CreateHomepageViewModel(_homepage);

        // then
        Assert.Collection(result.Cards,
            card => Assert.Equal(detailpage1.Name, card.Title),
            card => Assert.Equal(detailpage2.Name, card.Title));
    }
}