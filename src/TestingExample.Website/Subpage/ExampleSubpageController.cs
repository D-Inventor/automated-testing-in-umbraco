using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace TestingExample.Website.Subpage;

public class ExampleSubpageController(
    ILogger<RenderController> logger,
    ICompositeViewEngine compositeViewEngine,
    IUmbracoContextAccessor umbracoContextAccessor,
    ExampleSubpageRequestHandler requestHandler)
    : RenderController(logger, compositeViewEngine, umbracoContextAccessor)
{
    private readonly ExampleSubpageRequestHandler _requestHandler = requestHandler;

    public IActionResult ExampleSubpage()
    {
        return CurrentPage is ExampleSubpage subPage
            ? CurrentTemplate(_requestHandler.CreateSubpageViewModel(subPage))
            : NotFound();
    }
}
