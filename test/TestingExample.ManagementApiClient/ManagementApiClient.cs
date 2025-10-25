namespace TestingExample.ManagementApiClient;

public interface IDocumentTypeClient
{
    Task<Guid> GetDocumentTypeAsync(string alias, CancellationToken cancellationToken);
}

public partial class Document_TypeClient : IDocumentTypeClient
{
    public async Task<Guid> GetDocumentTypeAsync(string alias, CancellationToken cancellationToken)
    {
        var response = await GetItemDocumentTypeSearchAsync(alias, isElement: null, skip: 0, take: 1, cancellationToken: cancellationToken);
        return response.Items.FirstOrDefault()?.Id ?? throw new ArgumentException("Document type with alias '" + alias + "' does not exist", nameof(alias));
    }
}

public interface ITemplateClient
{
    Task<Guid> GetTemplateAsync(string alias, CancellationToken cancellationToken);
}

public partial class TemplateClient : ITemplateClient
{
    public async Task<Guid> GetTemplateAsync(string alias, CancellationToken cancellationToken)
    {
        var response = await GetItemTemplateSearchAsync(alias, skip: 0, take: 1, cancellationToken: cancellationToken);
        return response.Items.FirstOrDefault()?.Id ?? throw new ArgumentException("Template with alias '" + alias + "' does not exist", nameof(alias));
    }
}

public interface IDocumentClient
{
    Task PostDocumentAsync(CreateDocumentRequestModel? body, CancellationToken cancellationToken);
    Task ClearDocumentsAsync(CancellationToken cancellationToken);
    Task PutDocumentByIdPublishAsync(Guid id, PublishDocumentRequestModel? body, CancellationToken cancellationToken);
    Task PutDocumentByIdDomainsAsync(Guid id, UpdateDomainsRequestModel? body, CancellationToken cancellationToken);
    Task<ICollection<DocumentUrlInfoResponseModel>> GetDocumentUrlsAsync(IEnumerable<Guid>? id, CancellationToken cancellationToken);
}

public partial class DocumentClient : IDocumentClient
{
    public Task ClearDocumentsAsync(CancellationToken cancellationToken)
        => ClearAsync(cancellationToken);
}