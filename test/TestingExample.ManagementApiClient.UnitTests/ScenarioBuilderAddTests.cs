
using TestingExample.ManagementApiClient.Scenario;
using TestingExample.ManagementApiClient.Scenario.Model;
using TestingExample.ManagementApiClient.UnitTests.TestDoubles;

namespace TestingExample.ManagementApiClient.UnitTests;

/* Test cases:
 * - Builder creates the page
 * - Builder stores the page
 * - Builder assigns parent if specified
 * - Builder assigns template if specified
 * - Builder assigns content type
 * - Builder assigns id if specified
 */
public class ScenarioBuilderAddTests
{
    private static readonly Guid DefaultContentType = ExampleContentTypes.DefaultContentType;
    private readonly ScenarioBuilder _scenario;

    public ScenarioBuilderAddTests()
    {
        _scenario = new(new Uri("https://example.com"), new DocumentClientDummy());
    }

    [Fact]
    public void ShouldCreateThePage()
    {
        // when
        var page = _scenario.Add<ExampleContentModel>(contentType: DefaultContentType);

        // then
        Assert.NotNull(page);
    }

    [Fact]
    public void ShouldStoreThePage()
    {
        // given
        var page = _scenario.Add<ExampleContentModel>(contentType: DefaultContentType);

        // when
        var result = _scenario.Get(page.Id);

        // then
        Assert.Same(page, result);
    }

    [Fact]
    public void ShouldAssignContentType()
    {
        // when
        var page = _scenario.Add<ExampleContentModel>(contentType: Guid.Parse("8901b510-1259-4c4b-b2b0-80c28d70b1d3"));

        // then
        Assert.Equal(Guid.Parse("8901b510-1259-4c4b-b2b0-80c28d70b1d3"), page.ContentType);
    }

    [Fact]
    public void ShouldAssignParent()
    {
        // given
        var parent = _scenario.Add<ExampleContentModel>(contentType: DefaultContentType);

        // when
        var result = _scenario.Add<ExampleContentModel>(parent: parent.Id, contentType: DefaultContentType);

        // then
        Assert.Same(parent, result.Parent);
    }

    [Fact]
    public void ShouldAssignTemplate()
    {
        // when
        var result = _scenario.Add<ExampleContentModel>(template: Guid.Parse("37503562-fa77-4ba4-9b44-75d5c3096bec"), contentType: DefaultContentType);

        // then
        Assert.Equal(Guid.Parse("37503562-fa77-4ba4-9b44-75d5c3096bec"), result.Template);
    }

    [Fact]
    public void ShouldAssignId()
    {
        // when
        var result = _scenario.Add<ExampleContentModel>(id: Guid.Parse("ca55e129-96af-4c4c-a4e8-de55df3a6ec1"), contentType: DefaultContentType);

        // then
        Assert.Equal(Guid.Parse("ca55e129-96af-4c4c-a4e8-de55df3a6ec1"), result.Id);
    }

    private class ExampleContentModel(VariationModel variation, PageModel<ExampleContentModel> page)
        : ContentModel<ExampleContentModel>(variation, page);
}