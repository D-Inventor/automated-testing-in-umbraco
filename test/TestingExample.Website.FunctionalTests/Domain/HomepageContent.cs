using TestingExample.ManagementApiClient.Scenario.Model;

namespace TestingExample.Website.FunctionalTests.Domain;

public class HomepageContent(VariationModel variation, PageModel<HomepageContent> page)
    : ContentModel<HomepageContent>(variation, page)
{
    public static Guid ContentType { get; } = Guid.Parse("f0cf962b-6398-477c-aa04-e4fbb4d69162");
    public static Guid Template { get; } = Guid.Parse("b1ac1c58-672a-4645-afe8-b629e7e204b5");

    public HomepageContent WithHeader(string title)
        => this.SetValue("title", title);
}
