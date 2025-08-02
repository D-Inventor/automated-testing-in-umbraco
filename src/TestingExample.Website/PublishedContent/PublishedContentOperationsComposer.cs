using Umbraco.Cms.Core.Composing;

namespace TestingExample.Website.PublishedContent;

public class PublishedContentOperationsComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddSingleton<IPublishedContentOperations, PublishedContentOperations>();
    }
}
