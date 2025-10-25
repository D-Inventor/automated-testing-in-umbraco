using System.Globalization;

namespace TestingExample.ManagementApiClient.Scenario.Model;

public static class ContentModifications
{
    public static T SetValue<T>(this T content, string key, object? value)
        where T : ContentModel<T>
        => content.SetValue(content.Variation, key, value);

    public static T SetInvariantValue<T>(this T content, string key, object? value)
        where T : ContentModel<T>
        => content.SetValue(Variation.Invariant, key, value);

    private static T SetValue<T>(this T content, VariationType variation, string key, object? value)
        where T : ContentModel<T>
    {
        content.Page.SetValue(new ValueModel(variation, key, value));
        return content;
    }

    public static T HasVariation<T>(this T page, VariationType variation, string name)
        where T : PageModel
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        page.AddVariation(new VariationModel(variation, name));
        return page;
    }

    public static T IsPublished<T>(this T content)
        where T : ContentModel
    {
        content.Publish();
        return content;
    }

    public static T HasDomain<T>(this T page, CultureInfo culture, string domain)
        where T : PageModel
    {
        page.AddDomain(new DomainModel(culture, domain));
        return page;
    }
}
