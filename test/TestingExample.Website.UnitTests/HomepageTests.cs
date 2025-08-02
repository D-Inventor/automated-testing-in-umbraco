using NSubstitute;
using TestingExample.Website.Homepage;
using TestingExample.Website.UnitTests.PublishedContent;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace TestingExample.Website.UnitTests
{
    public class HomepageTests
    {
        private readonly ExampleWebsite _homepage;
        private readonly FakePublishedContentOperations _publishedContentOperations = new ();
        private readonly ExampleWebsiteRequestHandler _requestHandler;

        public HomepageTests()
        {
            _homepage = FakePublishedContent.Generate<ExampleWebsite>();
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
        public void ShouldCreateCardsForSubPages()
        {
            // given
            var subPage1 = FakePublishedContent.Generate<ExampleSubpage>();
            var subPage2 = FakePublishedContent.Generate<ExampleSubpage>();
            _publishedContentOperations.SetChildren(_homepage, [subPage1, subPage2]);

            // when
            var result = _requestHandler.CreateHomepageViewModel(_homepage);

            // then
            Assert.Collection(result.Cards,
                card => Assert.Equal(subPage1.Name, card.Title),
                card => Assert.Equal(subPage2.Name, card.Title));
        }
    }
}