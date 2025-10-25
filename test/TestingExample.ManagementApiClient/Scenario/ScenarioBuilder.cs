using TestingExample.ManagementApiClient.Scenario.Model;

namespace TestingExample.ManagementApiClient.Scenario;

public class ScenarioBuilder(Uri baseUri, IDocumentClient documentClient)
{
    private readonly Dictionary<Guid, PageModel> _pages = [];
    private readonly IDocumentClient _documentClient = documentClient;

    public Uri BaseUri { get; } = baseUri;

    public PageModel<T> Add<T>(Guid contentType, Guid? id = null, Guid? parent = null, Guid? template = null)
        where T : ContentModel<T> 
        => Add(new PageModel<T>(contentType, id)
        {
            Parent = parent.HasValue ? Get(parent.Value) : null,
            Template = template
        });

    public PageModel<T> Add<T>(PageModel<T> page)
        where T : ContentModel<T>
    {
        _pages.Add(page.Id, page);
        return page;
    }

    public async Task BuildAsync(CancellationToken cancellationToken = default)
    {
        await _documentClient.ClearDocumentsAsync(cancellationToken);

        foreach (var page in _pages.Values.OrderBy(page => page.Level))
        {
            await _documentClient.PostDocumentAsync(page.MapToCreateDocumentRequest(), cancellationToken);

            if (page.Domains.Any()) await _documentClient.PutDocumentByIdDomainsAsync(page.Id, page.MapToUpdateDomainsRequest(), cancellationToken);
        }

        foreach (var page in _pages.Values
            .Where(page => page.Variations.Any(variation => variation.Published))
            .OrderBy(page => page.Level))
        {
            await _documentClient.PutDocumentByIdPublishAsync(page.Id, page.MapToPublishDocumentRequest(), cancellationToken);
        }

        var urlInfos = await _documentClient.GetDocumentUrlsAsync(_pages.Values.Select(page => page.Id), cancellationToken);
        foreach (var urlInfo in urlInfos)
        {
            if (_pages.TryGetValue(urlInfo.Id, out var page))
            {
                foreach (var url in urlInfo.UrlInfos)
                {
                    var uri = Uri.TryCreate(url.Url, UriKind.Absolute, out var parsedUri) ? parsedUri
                        : new Uri(BaseUri, url.Url);

                    var locale = url.Culture is not null ? Locale.Culture(url.Culture) : Locale.Invariant;
                    page.SetUrl(new UrlModel(locale, uri));
                }
            }
        }
    }

    public PageModel Get(Guid id)
    {
        return _pages[id];
    }
}
