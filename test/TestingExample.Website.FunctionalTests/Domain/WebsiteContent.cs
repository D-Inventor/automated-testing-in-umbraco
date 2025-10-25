using TestingExample.ManagementApiClient.Scenario.Model;

namespace TestingExample.Website.FunctionalTests.Domain;

public class WebsiteContent(VariationModel variation, PageModel<WebsiteContent> page)
    : ContentModel<WebsiteContent>(variation, page)
{
    public static Guid ContentType { get; } = Guid.Parse("97001531-6692-4e9c-a64a-3295c86b981d");

    public WebsiteContent HasHomepage(Guid homepageId)
        => this.SetInvariantValue("umbracoInternalRedirectId", homepageId);
}
