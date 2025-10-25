using System.Globalization;

using TestingExample.ManagementApiClient.Scenario;
using TestingExample.ManagementApiClient.Scenario.Model;
using TestingExample.Website.FunctionalTests.Domain;

namespace TestingExample.Website.FunctionalTests;

public static class BaseContentScenario
{
    public static ScenarioBuilder WithBasicContent(this ScenarioBuilder builder)
    {
        var websiteRoot = builder.Add<WebsiteRootContent>(
            contentType: WebsiteRootContent.ContentType,
            id: DefaultContent.WebsiteRootId)
            .HasVariation(Variation.Invariant, "Corporate website", content => content.IsPublished());

        var website = builder.Add<WebsiteContent>(
            contentType: WebsiteContent.ContentType,
            id: DefaultContent.WebsiteId,
            parent: websiteRoot.Id)
            .HasVariation(Variation.Invariant, "Website", content => content
                .HasHomepage(DefaultContent.HomepageId)
                .IsPublished())
            .HasDomain(CultureInfo.GetCultureInfo("en-US"), builder.BaseUri.Host + (builder.BaseUri.IsDefaultPort ? string.Empty : ":" + builder.BaseUri.Port));

        builder.Add<HomepageContent>(
            contentType: HomepageContent.ContentType,
            id: DefaultContent.HomepageId,
            parent: website.Id,
            template: HomepageContent.Template)
            .HasVariation(Variation.Invariant, "Homepage", content => content.IsPublished());

        return builder;
    }

    public static PageModel<WebsiteContent> Website(this ScenarioBuilder builder)
        => builder.Get(DefaultContent.WebsiteId) as PageModel<WebsiteContent> ?? throw new InvalidOperationException("There is no website registered in this scenario yet.");

    public static PageModel<HomepageContent> Homepage(this ScenarioBuilder builder)
        => builder.Get(DefaultContent.HomepageId) as PageModel<HomepageContent> ?? throw new InvalidOperationException("There is no homepage registered in this scenario yet.");
}
