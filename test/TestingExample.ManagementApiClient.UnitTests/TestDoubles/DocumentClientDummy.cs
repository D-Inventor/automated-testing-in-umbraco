
namespace TestingExample.ManagementApiClient.UnitTests.TestDoubles;

public class DocumentClientDummy : IDocumentClient
{
    public Task ClearDocumentsAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<ICollection<DocumentUrlInfoResponseModel>> GetDocumentUrlsAsync(IEnumerable<Guid>? id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task PostDocumentAsync(CreateDocumentRequestModel? body, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task PutDocumentByIdDomainsAsync(Guid id, UpdateDomainsRequestModel? body, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task PutDocumentByIdPublishAsync(Guid id, PublishDocumentRequestModel? body, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}