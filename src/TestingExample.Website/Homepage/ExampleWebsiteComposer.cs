using Umbraco.Cms.Core.Composing;

namespace TestingExample.Website.Homepage;

public class ExampleWebsiteComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<ExampleWebsiteRequestHandler>();
    }
}
