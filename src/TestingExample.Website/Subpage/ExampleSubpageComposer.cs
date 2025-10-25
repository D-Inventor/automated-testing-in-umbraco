using Umbraco.Cms.Core.Composing;

namespace TestingExample.Website.Subpage;

public class ContentPageComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<ContentPageRequestHandler>();
    }
}
