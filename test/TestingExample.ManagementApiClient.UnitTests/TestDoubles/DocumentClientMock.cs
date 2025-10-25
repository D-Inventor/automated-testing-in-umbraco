
namespace TestingExample.ManagementApiClient.UnitTests.TestDoubles;

public class DocumentClientMock() : IDocumentClient
{
    private readonly Dictionary<Guid, DocumentUrlInfoResponseModel> _responses = [];

    public DocumentClientMock WithUrlFor(Guid pageId, string? culture, string url)
    {
        var response = _responses.TryGetValue(pageId, out var item) ? item : new DocumentUrlInfoResponseModel(pageId, []);
        _responses[pageId] = response with { UrlInfos = [.. response.UrlInfos, new DocumentUrlInfoModel(culture, url)] };

        return this;
    }

    public Task ClearDocumentsAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task<ICollection<DocumentUrlInfoResponseModel>> GetDocumentUrlsAsync(IEnumerable<Guid>? id, CancellationToken cancellationToken)
    {
        return Task.FromResult<ICollection<DocumentUrlInfoResponseModel>>([.. (id ?? []).Aggregate(
            Enumerable.Empty<DocumentUrlInfoResponseModel>(),
            (acc, item) => _responses.TryGetValue(item, out var value) ? acc.Append(value) : acc
        )]);
    }

    public Task PostDocumentAsync(CreateDocumentRequestModel? body, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task PutDocumentByIdDomainsAsync(Guid id, UpdateDomainsRequestModel? body, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task PutDocumentByIdPublishAsync(Guid id, PublishDocumentRequestModel? body, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
