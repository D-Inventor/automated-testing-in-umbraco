using Umbraco.Cms.Core.Models.PublishedContent;

namespace TestingExample.Website.UnitTests.PublishedContent;

internal sealed class FakePublishedProperty(string alias, object? value)
    : IPublishedProperty
{
    public IPublishedPropertyType PropertyType => throw new NotImplementedException();

    public string Alias { get; } = alias;

    public object? GetDeliveryApiValue(bool expanding, string? culture = null, string? segment = null)
    {
        return value;
    }

    public object? GetSourceValue(string? culture = null, string? segment = null)
    {
        return value;
    }

    public object? GetValue(string? culture = null, string? segment = null)
    {
        return value;
    }

    public bool HasValue(string? culture = null, string? segment = null)
    {
        return value is not null;
    }
}
