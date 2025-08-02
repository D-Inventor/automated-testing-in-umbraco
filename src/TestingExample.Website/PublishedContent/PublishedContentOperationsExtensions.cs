using Umbraco.Cms.Core.Models.PublishedContent;

namespace TestingExample.Website.PublishedContent;

public static class PublishedContentOperationsExtensions
{
    public static Uri? Url(this IPublishedContent content, IPublishedContentOperations operations)
        => operations.Url(content);

    public static IEnumerable<T> Children<T>(this IPublishedContent content, IPublishedContentOperations operations)
        where T : class, IPublishedContent
        => operations.Children<T>(content);

    public static T? Parent<T>(this IPublishedContent content, IPublishedContentOperations operations)
        where T : class, IPublishedContent
        => operations.Parent<T>(content);
}
