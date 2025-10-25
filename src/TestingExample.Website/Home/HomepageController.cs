using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;

using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace TestingExample.Website.Home;

public class HomepageController(
    ILogger<RenderController> logger,
    ICompositeViewEngine compositeViewEngine,
    IUmbracoContextAccessor umbracoContextAccessor,
    HomepageRequestHandler requestHandler)
    : RenderController(logger, compositeViewEngine, umbracoContextAccessor)
{
    private readonly HomepageRequestHandler _requestHandler = requestHandler;

    public IActionResult Homepage()
    {
        return CurrentPage is Homepage Homepage
            ? CurrentTemplate(_requestHandler.CreateHomepageViewModel(Homepage))
            : NotFound();
    }
}
