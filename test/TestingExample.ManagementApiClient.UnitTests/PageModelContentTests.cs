using TestingExample.ManagementApiClient.Scenario.Model;

namespace TestingExample.ManagementApiClient.UnitTests;

/* Test cases
 * - ✅ Creates variation
 * - ✅ Creates variation with name
 * - ✅ Cannot create variation without name
 * - ✅ Saves variation property values
 * - ✅ Saves invariant property values through variation
 * - ✅ Can configure created variation
 * - ✅ Can change the name of a variation
 * - ✅ Must create variation before configuring
 * - Can publish a variation
 */
public class ContentModelTests
{
    private readonly PageModel<ContentTestModel> _page = new(contentType: Guid.NewGuid());

    [Fact]
    public void ShouldCreateContentWithVariation()
    {
        // when
        _page.HasVariation(Variation.Culture("nl-NL"), "Dutch content");

        // then
        Assert.Contains(_page.Variations, content => content.Variation == Variation.Culture("nl-NL"));
    }

    [Fact]
    public void ShouldCreateVariationWithName()
    {
        // when
        _page.HasVariation(Variation.Invariant, "Invariant content");

        // then
        Assert.Equal("Invariant content", _page.Variations.Single(content => content.Variation == Variation.Invariant).Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void ShouldNotCreateVariationWithoutName(string? input)
    {
        Assert.ThrowsAny<ArgumentException>(() => _page.HasVariation(Variation.Invariant, input!));
    }

    [Fact]
    public void ShouldStoreValuesThatVary()
    {
        // when
        _page.HasVariation(Variation.Culture("en-US"), "English culture", content => content.SetTitle(title: "English header title"));

        // then
        var propertyValue = Assert.Single(_page.Values, value => value.Alias == "title");
        Assert.Equal(Variation.Culture("en-US"), propertyValue.Variation);
        Assert.Equal("English header title", propertyValue.Value);
    }

    [Fact]
    public void ShouldStoreInvariantValuesThroughVariation()
    {
        // when
        _page.HasVariation(Variation.Culture("en-US"), "English culture", content => content.SetHeaderImage("Demo image"));

        // then
        var propertyValue = Assert.Single(_page.Values, value => value.Alias == "headerImage");
        Assert.Equal(Variation.Invariant, propertyValue.Variation);
        Assert.Equal("Demo image", propertyValue.Value);
    }

    [Fact]
    public void ShouldExposeVariationsAfterCreating()
    {
        // given
        _page.HasVariation(Variation.Culture("en-US"), "English variation");

        // when
        _page.HasContent(Variation.Culture("en-US"), content => content.SetTitle("Hello world"));

        // then
        var propertyValue = Assert.Single(_page.Values, value => value.Alias == "title" && value.Variation == Variation.Culture("en-US"));
    }

    [Fact]
    public void ShouldChangeNameOfVariationAfterCreating()
    {
        // given
        _page.HasVariation(Variation.Culture("en-US"), "English variation");

        // when
        _page.HasContent(Variation.Culture("en-US"), content => content.SetName("Name is overwritten"));

        // then
        Assert.Equal("Name is overwritten", _page.Variations.Single(content => content.Variation == Variation.Culture("en-US")).Name);
    }

    [Fact]
    public void ShouldOnlyConfigureVariationsThatThePageHas()
    {
        var exception = Assert.Throws<InvalidOperationException>(() => _page.HasContent(Variation.Culture("de-DE"), _ => { }));
        Assert.Equal("You must call 'HasVariation()' for the given variation before using 'HasContent()'", exception.Message);
    }

    [Fact]
    public void ShouldPublishVariation()
    {
        // when
        _page.HasVariation(Variation.Culture("nl-NL"), "Nederlandse variant", content => content.IsPublished());

        // then
        Assert.True(_page.Variations.Single(variation => variation.Variation == Variation.Culture("nl-NL")).Published);
    }

    private class ContentTestModel(VariationModel variation, PageModel<ContentTestModel> page)
        : ContentModel<ContentTestModel>(variation, page)
    {
        public ContentTestModel SetTitle(string title) => this.SetValue("title", title);

        public ContentTestModel SetHeaderImage(string imageName) => this.SetInvariantValue("headerImage", imageName);
    }
}