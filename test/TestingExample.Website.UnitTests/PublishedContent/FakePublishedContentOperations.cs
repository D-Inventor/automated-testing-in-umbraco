using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using TestingExample.Website.PublishedContent;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace TestingExample.Website.UnitTests.PublishedContent;

internal class FakePublishedContentOperations : IPublishedContentOperations
{
    private readonly ConcurrentDictionary<IPublishedContent, List<IPublishedContent>> _parentToChildrenMap = new(new PublishedContentEqualityComparer());
    private readonly ConcurrentDictionary<IPublishedContent, Uri> _urlMap = new(new PublishedContentEqualityComparer());

    public void SetChildren(IPublishedContent parent, IEnumerable<IPublishedContent> children)
    {
        foreach (var child in children)
        {
            SetParent(child, parent);
        }
    }

    public void SetParent(IPublishedContent child, IPublishedContent parent)
    {
        RemoveChildFromAllParents(child);
        AddChildToParent(child, parent);
    }

    private void AddChildToParent(IPublishedContent child, IPublishedContent parent)
    {
        if (!_parentToChildrenMap.TryGetValue(parent, out var childrenList))
        {
            childrenList = [];
            _parentToChildrenMap[parent] = childrenList;
        }

        childrenList.Add(child);
    }

    private void RemoveChildFromAllParents(IPublishedContent child)
    {
        foreach (var childrenList in _parentToChildrenMap.Values)
        {
            childrenList.Remove(child);
        }
    }

    public void SetUrl(IPublishedContent content, Uri url)
        => _urlMap[content] = url;

    public IEnumerable<T> Children<T>(IPublishedContent content)
        where T : class, IPublishedContent
        => _parentToChildrenMap.TryGetValue(content, out var children)
        ? children.OfType<T>()
        : [];

    public T? Parent<T>(IPublishedContent content)
        where T : class, IPublishedContent
    {
        foreach ((var parent, var children) in _parentToChildrenMap)
        {
            if (children.Contains(content))
            {
                return parent as T;
            }
        }
        return null;
    }

    public Uri? Url(IPublishedContent content)
        => _urlMap.TryGetValue(content, out var url)
        ? url
        : null;

    private class PublishedContentEqualityComparer : IEqualityComparer<IPublishedContent>
    {
        public bool Equals(IPublishedContent? x, IPublishedContent? y)
            => x is not null && y is not null && x.Id == y.Id;

        public int GetHashCode([DisallowNull] IPublishedContent obj)
            => HashCode.Combine(obj.Id);
    }
}
