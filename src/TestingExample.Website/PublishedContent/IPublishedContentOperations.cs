using Umbraco.Cms.Core.Models.PublishedContent;

namespace TestingExample.Website.PublishedContent;

public interface IPublishedContentOperations
{
    IEnumerable<T> Children<T>(IPublishedContent content)
        where T : class, IPublishedContent;

    T? Parent<T>(IPublishedContent content)
        where T : class, IPublishedContent;

    Uri? Url(IPublishedContent content);
}