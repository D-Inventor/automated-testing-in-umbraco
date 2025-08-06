using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Management.Controllers;
using Umbraco.Cms.Api.Management.Routing;

namespace TestingExample.Website.BackofficeApi;

[VersionedApiBackOfficeRoute("exampleapi")]
[ApiExplorerSettings(GroupName = "Example Backoffice API")]
public class ExampleBackofficeApiController
    : ManagementApiControllerBase
{
    [HttpGet]
    public IActionResult GetResource()
    {
        return Ok(new { Message = "This is an example response from the backoffice API." });
    }
}