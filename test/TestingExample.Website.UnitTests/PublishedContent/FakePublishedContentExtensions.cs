using Umbraco.Cms.Core.Models.PublishedContent;
using static TestingExample.Website.UnitTests.PublishedContent.FakePublishedElement;

namespace TestingExample.Website.UnitTests.PublishedContent;

internal static class FakePublishedContentExtensions
{
    public static TContent WithPropertyValues<TContent>(this TContent content, Action<PropertyValueBuilder<TContent>> config)
        where TContent : IPublishedElement
    {
        config(content.PropertyValues());
        return content;
    }

    public static PropertyValueBuilder<TContent> PropertyValues<TContent>(this TContent content)
        where TContent : IPublishedElement
        => content switch
        {
            FakePublishedElement el => el.PropertyValuesFor<TContent>(),
            PublishedContentWrapped el => el.PropertyValuesFor<TContent>(),
            PublishedElementWrapped el => el.PropertyValuesFor<TContent>(),
            _ => throw new InvalidOperationException("Cannot create property value builder from the given content item")
        };

    private static PropertyValueBuilder<TContent> PropertyValuesFor<TContent>(this PublishedContentWrapped content)
        where TContent : IPublishedElement
        => content.Unwrap() switch
        {
            FakePublishedElement fakeElement => fakeElement.PropertyValuesFor<TContent>(),
            PublishedContentWrapped wrapper => wrapper.PropertyValuesFor<TContent>(),
            _ => throw new InvalidOperationException("Cannot get property value builder, because this object is not based on fake content")
        };

    private static PropertyValueBuilder<TContent> PropertyValuesFor<TContent>(this PublishedElementWrapped content)
        where TContent : IPublishedElement
        => content.Unwrap() switch
        {
            FakePublishedElement fakeElement => fakeElement.PropertyValuesFor<TContent>(),
            PublishedElementWrapped wrapper => wrapper.PropertyValuesFor<TContent>(),
            _ => throw new InvalidOperationException("Cannot get property value builder, because this object is not based on fake content")
        };
}
