using NSubstitute;

using TestingExample.Website.Subpage;
using TestingExample.Website.UnitTests.PublishedContent;

using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace TestingExample.Website.UnitTests;

public class ContentPageTests
{
    private readonly ContentPage _subPage;
    private readonly FakePublishedContentOperations _contentOperations;
    private readonly ContentPageRequestHandler _requestHandler;

    public ContentPageTests()
    {
        _subPage = FakePublishedContent.Generate<ContentPage>();
        _contentOperations = new FakePublishedContentOperations();
        _requestHandler = new ContentPageRequestHandler(Substitute.For<IPublishedValueFallback>(), _contentOperations);
    }

    [Fact]
    public void ShouldCreateAViewModel()
    {
        // given / when
        var result = _requestHandler.CreateSubpageViewModel(_subPage);

        // then
        Assert.NotNull(result);
    }

    [Fact]
    public void ShouldCreateLinkToParentPage()
    {
        // given
        var parentPage = FakePublishedContent.Generate<Homepage>();
        _contentOperations.SetUrl(parentPage, new Uri("http://example.com"));
        _contentOperations.SetParent(_subPage, parentPage);

        // when
        var result = _requestHandler.CreateSubpageViewModel(_subPage);

        // then
        Assert.NotNull(result.ParentLink);
        Assert.Equal(new Uri("http://example.com"), result.ParentLink.Url);
    }

    [Fact]
    public void ShouldSelectNameAsSEOTitle()
    {
        // given / when
        var seoTitle = _subPage.GetSEOTitle();

        // then
        Assert.Equal(_subPage.Name, seoTitle);
    }

    [Fact]
    public void ShouldPreferSEOTitleOverName()
    {
        // given
        _subPage.PropertyValues()
            .Set(content => content.SeoTitle, "Custom SEO Title");

        // when
        var result = _subPage.GetSEOTitle();

        // then
        Assert.Equal("Custom SEO Title", result);
    }
}
