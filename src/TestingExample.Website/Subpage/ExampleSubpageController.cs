using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace TestingExample.Website.Subpage;

public class ContentPageController(
    ILogger<RenderController> logger,
    ICompositeViewEngine compositeViewEngine,
    IUmbracoContextAccessor umbracoContextAccessor,
    ContentPageRequestHandler requestHandler)
    : RenderController(logger, compositeViewEngine, umbracoContextAccessor)
{
    private readonly ContentPageRequestHandler _requestHandler = requestHandler;

    public IActionResult ContentPage()
    {
        return CurrentPage is ContentPage subPage
            ? CurrentTemplate(_requestHandler.CreateSubpageViewModel(subPage))
            : NotFound();
    }
}
