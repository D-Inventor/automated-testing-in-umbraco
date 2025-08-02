using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace TestingExample.Website.Homepage;

public class ExampleWebsiteController(
    ILogger<RenderController> logger,
    ICompositeViewEngine compositeViewEngine,
    IUmbracoContextAccessor umbracoContextAccessor,
    ExampleWebsiteRequestHandler requestHandler)
    : RenderController(logger, compositeViewEngine, umbracoContextAccessor)
{
    private readonly ExampleWebsiteRequestHandler _requestHandler = requestHandler;

    public IActionResult ExampleWebsite()
    {
        return CurrentPage is ExampleWebsite exampleWebsite
        && _requestHandler.CreateHomepageViewModel(exampleWebsite) is { } viewModel
            ? CurrentTemplate(viewModel)
            : NotFound();}
}
