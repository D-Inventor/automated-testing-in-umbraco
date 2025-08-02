using Umbraco.Cms.Core.Composing;

namespace TestingExample.Website.Subpage;

public class ExampleSubpageComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<ExampleSubpageRequestHandler>();
    }
}
