using Microsoft.AspNetCore.Mvc;

using Umbraco.Cms.Api.Management.Controllers;
using Umbraco.Cms.Api.Management.Routing;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;

namespace TestingExample.Website.Testing;

[VersionedApiBackOfficeRoute("scenario/clear")]
[ApiExplorerSettings(GroupName = "Document")]
public class ClearScenarioController(
    IContentService contentService,
    IDomainService domainService,
    IIdKeyMap idKeyMap,
    ICoreScopeProvider scopeProvider)
    : ManagementApiControllerBase
{
    private readonly IContentService _contentService = contentService;
    private readonly IDomainService _domainService = domainService;
    private readonly IIdKeyMap _idKeyMap = idKeyMap;
    private readonly ICoreScopeProvider _scopeProvider = scopeProvider;

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ClearAsync()
    {
        using var scope = _scopeProvider.CreateCoreScope();

        // Clear all domains explicitly (this allows domain cache to update)
        var domains = await _domainService.GetAllAsync(includeWildcards: true);

        var contentKeys = domains
            .Select(domain => domain.RootContentId)
            .OfType<int>()
            .Distinct()
            .Select(id => _idKeyMap.GetKeyForId(id, UmbracoObjectTypes.Document))
            .Where(keyAttempt => keyAttempt.Success)
            .Select(keyAttempt => keyAttempt.Result);

        foreach (var contentKey in contentKeys)
        {
            await _domainService.UpdateDomainsAsync(contentKey, new DomainsUpdateModel { Domains = [] });
        }

        // Delete all root content
        foreach (var rootContent in _contentService.GetRootContent())
        {
            _contentService.Delete(rootContent, User.GetUmbracoIdentity()?.GetId() ?? Constants.System.Root);
        }

        scope.Complete();
        return NoContent();
    }
}
