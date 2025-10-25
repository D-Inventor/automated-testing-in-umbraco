using TestingExample.ManagementApiClient.Scenario.Model;

namespace TestingExample.Website.FunctionalTests.Domain;

public class WebsiteRootContent(VariationModel variation, PageModel<WebsiteRootContent> page)
    : ContentModel<WebsiteRootContent>(variation, page)
{
    public static Guid ContentType { get; } = Guid.Parse("92b91c59-bce3-42ec-af97-8d1f22722fc5");
}
