using Umbraco.Cms.Core.Composing;

namespace TestingExample.Website.Home;

public class HomepageComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<HomepageRequestHandler>();
    }
}
