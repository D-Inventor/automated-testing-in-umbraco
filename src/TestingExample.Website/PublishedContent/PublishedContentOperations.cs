using Umbraco.Cms.Core.Models.PublishedContent;

namespace TestingExample.Website.PublishedContent
{
    public class PublishedContentOperations : IPublishedContentOperations
    {
        public IEnumerable<T> Children<T>(IPublishedContent content)
            where T : class, IPublishedContent
            => content.Children<T>() ?? [];

        public T? Parent<T>(IPublishedContent content)
            where T : class, IPublishedContent
            => content.Parent<T>();

        public Uri? Url(IPublishedContent content)
        {
            var url = content.Url();
            return !string.IsNullOrWhiteSpace(url) && Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri) ? uri
                : null;
        }
    }
}
