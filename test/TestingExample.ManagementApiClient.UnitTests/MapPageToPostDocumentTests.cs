using TestingExample.ManagementApiClient.Scenario;
using TestingExample.ManagementApiClient.Scenario.Model;

namespace TestingExample.ManagementApiClient.UnitTests;

/* Test cases:
 * - Maps Id, content type, template
 * - Maps parent
 * - Maps variants
 * - Maps property values
 */
public class MapPageToPostDocumentTests
{
    [Fact]
    public void ShouldMapIdContentTypeAndTemplate()
    {
        // given
        var page = new PageModel<ExampleContentModel>(
            contentType: Guid.Parse("71cdab9b-7727-498d-9bf3-85998e278938"),
            id: Guid.Parse("cc69407b-fa66-46ff-b71c-a5332c3758ce"))
        {
            Template = Guid.Parse("abc12a99-e5cb-4415-8209-6686d6310653")
        };

        // when
        var result = page.MapToCreateDocumentRequest();

        // then
        Assert.Equal(Guid.Parse("cc69407b-fa66-46ff-b71c-a5332c3758ce"), result.Id);
        Assert.Equal(Guid.Parse("71cdab9b-7727-498d-9bf3-85998e278938"), result.DocumentType.Id);
        Assert.Equal(Guid.Parse("abc12a99-e5cb-4415-8209-6686d6310653"), result.Template?.Id);
    }

    [Fact]
    public void ShouldMapParent()
    {
        // given
        var parent = new PageModel<ExampleContentModel>(id: Guid.Parse("a4f7376f-d4c6-4d26-8694-1e382666ff6a"), contentType: Guid.NewGuid());
        var page = new PageModel<ExampleContentModel>(contentType: Guid.NewGuid())
        {
            Parent = parent
        };

        // when
        var result = page.MapToCreateDocumentRequest();

        // then
        Assert.Equal(Guid.Parse("a4f7376f-d4c6-4d26-8694-1e382666ff6a"), result.Parent?.Id);
    }

    [Fact]
    public void ShouldMapVariations()
    {
        // given
        var page = new PageModel<ExampleContentModel>(contentType: Guid.NewGuid())
            .HasVariation(Variation.Culture("en-US"), "English variation");

        // when
        var result = page.MapToCreateDocumentRequest();

        // then
        var variation = Assert.Single(result.Variants);
        Assert.Equal("en-US", variation.Culture);
        Assert.Equal("English variation", variation.Name);
    }

    [Fact]
    public void ShouldMapPropertyValues()
    {
        // given
        var page = new PageModel<ExampleContentModel>(contentType: Guid.NewGuid())
            .HasVariation(Variation.Culture("nl-NL"), "Nederlandse variant", content => content.HasTitle("Voorbeeld titel"));

        // when
        var result = page.MapToCreateDocumentRequest();

        // then
        var value = Assert.Single(result.Values);
        Assert.Equal("nl-NL", value.Culture);
        Assert.Equal("Voorbeeld titel", value.Value);
    }

    private class ExampleContentModel(VariationModel variation, PageModel<ExampleContentModel> page)
        : ContentModel<ExampleContentModel>(variation, page)
    {
        public ExampleContentModel HasTitle(string title) => this.SetValue("title", title);
    }
}
