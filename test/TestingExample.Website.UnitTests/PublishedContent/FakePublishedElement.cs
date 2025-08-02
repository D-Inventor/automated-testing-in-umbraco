using System.Linq.Expressions;
using NSubstitute;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace TestingExample.Website.UnitTests.PublishedContent;

internal class FakePublishedElement(Guid? key = null)
    : IPublishedElement
{
    private readonly Dictionary<string, IPublishedProperty> _properties = [];

    public IPublishedContentType ContentType => throw new NotImplementedException();

    public Guid Key { get; } = key ?? Guid.NewGuid();

    public IEnumerable<IPublishedProperty> Properties => _properties.Values;

    public IPublishedProperty? GetProperty(string alias)
    {
        return _properties.TryGetValue(alias, out var prop)
            ? prop
            : null;
    }

    public T WrapIn<T>()
        where T : IPublishedElement
    {
        if (Activator.CreateInstance(typeof(T), this, Substitute.For<IPublishedValueFallback>()) is not T result)
            throw new InvalidOperationException($"Unable to create published content of type {typeof(T)}");

        return result;
    }

    public void AssignPropertyValue(string alias, object? value)
    {
        _properties[alias] = new FakePublishedProperty(alias, value);
    }

    public void UnassignPropertyValue(string alias)
    {
        _properties.Remove(alias);
    }

    public PropertyValueBuilder<TContent> PropertyValuesFor<TContent>()
        where TContent : IPublishedElement
        => new(this);

    public sealed class PropertyValueBuilder<TContent>(FakePublishedElement content)
        where TContent : IPublishedElement
    {
        public PropertyValueBuilder<TContent> Set<TProp>(Expression<Func<TContent, TProp?>> propertyExpression, TProp? value)
        {
            content.AssignPropertyValue(PropertyAliasHelper.AliasOf(propertyExpression), value);
            return this;
        }

        public PropertyValueBuilder<TContent> Unset<TProp>(Expression<Func<TContent, TProp?>> propertyExpression)
        {
            content.UnassignPropertyValue(PropertyAliasHelper.AliasOf(propertyExpression));
            return this;
        }
    }
}