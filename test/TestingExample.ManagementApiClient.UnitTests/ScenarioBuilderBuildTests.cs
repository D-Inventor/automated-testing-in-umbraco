using System.Globalization;

using TestingExample.ManagementApiClient.Scenario;
using TestingExample.ManagementApiClient.Scenario.Model;
using TestingExample.ManagementApiClient.UnitTests.TestDoubles;

namespace TestingExample.ManagementApiClient.UnitTests;

/* Test cases:
 * - Sends content pages to management api
 * - Sends variations to management api
 * - Sends property values to management api
 * - Sends parent pages before children
 * - Publishes published variants
 * - Deletes all old content
 * - Assigns urls to content after publish
 */
public class ScenarioBuilderBuildTests
{
    private static readonly Guid DefaultContentType = ExampleContentTypes.DefaultContentType;
    private readonly DocumentClientSpy _spy;
    private readonly ScenarioBuilder _scenario;

    public ScenarioBuilderBuildTests()
    {
        _spy = new DocumentClientSpy();
        _scenario = new ScenarioBuilder(new Uri("https://example.com"), _spy, new CacheClientStub());
    }

    [Fact]
    public async Task ShouldSavePage()
    {
        // given
        _scenario.Add<ExampleContentModel>(id: Guid.Parse("76316241-19a0-4405-87ed-ee848ae4f3ed"), contentType: DefaultContentType);

        // when
        await _scenario.BuildAsync(cancellationToken: TestContext.Current.CancellationToken);

        // then
        var page = Assert.Single(_spy.CreatedDocuments);
        Assert.Equal(Guid.Parse("76316241-19a0-4405-87ed-ee848ae4f3ed"), page.Id);
    }

    [Fact]
    public async Task ShouldPerformOperationsInTheCorrectOrder()
    {
        // given
        _scenario.Add<ExampleContentModel>(contentType: DefaultContentType)
            .HasVariation(Variation.Culture("en-US"), "English variation", content => content.IsPublished())
            .HasDomain(CultureInfo.GetCultureInfo("nl-NL"), "example.nl");

        // when
        await _scenario.BuildAsync(cancellationToken: TestContext.Current.CancellationToken);

        // then
        Assert.Collection(_spy.Operations,
            operation => Assert.Equal("clear", operation),
            operation => Assert.Equal("save", operation),
            operation => Assert.Equal("save domains", operation),
            operation => Assert.Equal("publish", operation));
    }

    [Fact]
    public async Task ShouldPublishVariants()
    {
        // given
        var page = _scenario.Add<ExampleContentModel>(contentType: DefaultContentType)
            .HasVariation(Variation.Culture("en-US"), "English variant", content => content.IsPublished())
            .HasVariation(Variation.Culture("nl-NL"), "Nederlandse variant");

        // when
        await _scenario.BuildAsync(cancellationToken: TestContext.Current.CancellationToken);

        // then
        var publishedVariant = Assert.Single(_spy.PublishedCulturesOfLastPage);
        Assert.Equal("en-US", publishedVariant);
    }

    [Fact]
    public async Task ShouldNotPublishIfNoVariantsArePublished()
    {
        // given
        var page = _scenario.Add<ExampleContentModel>(contentType: DefaultContentType)
            .HasVariation(Variation.Culture("en-US"), "English variant");

        // when
        await _scenario.BuildAsync(cancellationToken: TestContext.Current.CancellationToken);

        // then
        Assert.DoesNotContain("publish", _spy.Operations);
    }

    [Fact]
    public async Task ShouldSaveDomains()
    {
        // given
        var page = _scenario.Add<ExampleContentModel>(contentType: DefaultContentType)
            .HasDomain(CultureInfo.GetCultureInfo("en-US"), "example.com");

        // when
        await _scenario.BuildAsync(cancellationToken: TestContext.Current.CancellationToken);

        // then
        var domain = Assert.Single(_spy.LastCreatedDomains);
        Assert.Equal("en-US", domain.IsoCode);
        Assert.Equal("example.com", domain.DomainName);
    }

    [Fact]
    public async Task ShouldNotSaveDomainsIfNoneAreDefined()
    {
        // given
        _scenario.Add<ExampleContentModel>(contentType: DefaultContentType);

        // when
        await _scenario.BuildAsync(cancellationToken: TestContext.Current.CancellationToken);

        // then
        Assert.DoesNotContain("save domains", _spy.Operations);
    }

    [Fact]
    public async Task ShouldSaveParentsBeforeChildren()
    {
        // given
        var parent = new PageModel<ExampleContentModel>(contentType: DefaultContentType);
        var child = new PageModel<ExampleContentModel>(contentType: DefaultContentType)
        {
            Parent = parent
        };
        _scenario.Add(child);
        _scenario.Add(parent);

        // when
        await _scenario.BuildAsync(TestContext.Current.CancellationToken);

        // then
        Assert.Collection(_spy.SavedPages,
            page => Assert.Equal(parent.Id, page),
            page => Assert.Equal(child.Id, page));
    }

    [Fact]
    public async Task ShouldPublishParentsBeforeChildren()
    {
        // given
        var parent = new PageModel<ExampleContentModel>(contentType: DefaultContentType)
            .HasVariation(Variation.Invariant, "Parent content", content => content.IsPublished());
        var child = new PageModel<ExampleContentModel>(contentType: DefaultContentType)
        {
            Parent = parent
        }.HasVariation(Variation.Invariant, "Child content", content => content.IsPublished());
        _scenario.Add(child);
        _scenario.Add(parent);

        // when
        await _scenario.BuildAsync(TestContext.Current.CancellationToken);

        // then
        Assert.Collection(_spy.PublishedPages,
            page => Assert.Equal(parent.Id, page),
            page => Assert.Equal(child.Id, page));
    }

    [Fact]
    public async Task ShouldAssignUrlToContentAfterPublishing()
    {
        // given
        var pageId = Guid.Parse("1a235bf2-b566-4e83-8984-2cba3411601e");
        var scenario = new ScenarioBuilder(new Uri("https://example.com"), new DocumentClientMock()
            .WithUrlFor(pageId, "en-US", "https://example.com/"), new CacheClientStub());
        var page = scenario.Add<ExampleContentModel>(id: pageId, contentType: DefaultContentType)
            .HasVariation(Variation.Culture("en-US"), "English variant");

        // when
        await scenario.BuildAsync(TestContext.Current.CancellationToken);

        // then
        Assert.Equal(new Uri("https://example.com/"), page.Url(Locale.Culture("en-US")));
    }

    [Fact]
    public async Task ShouldInferAbsoluteUrlWithBaseUrl()
    {
        // given
        var pageId = Guid.Parse("1a235bf2-b566-4e83-8984-2cba3411601e");
        var scenario = new ScenarioBuilder(new Uri("https://example.com"), new DocumentClientMock()
            .WithUrlFor(pageId, "en-US", "/privacy-policy"), new CacheClientStub());
        var page = scenario.Add<ExampleContentModel>(id: pageId, contentType: DefaultContentType)
            .HasVariation(Variation.Culture("en-US"), "English variant");

        // when
        await scenario.BuildAsync(TestContext.Current.CancellationToken);

        // then
        Assert.Equal(new Uri("https://example.com/privacy-policy"), page.Url(Locale.Culture("en-US")));
    }

    private class ExampleContentModel(VariationModel variation, PageModel<ExampleContentModel> page)
        : ContentModel<ExampleContentModel>(variation, page);
}