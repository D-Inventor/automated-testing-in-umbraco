


namespace TestingExample.ManagementApiClient.UnitTests.TestDoubles;

public class DocumentClientSpy : IDocumentClient
{
    public List<CreateDocumentRequestModel> CreatedDocuments { get; } = [];
    public List<string> Operations { get; } = [];
    public List<string?> PublishedCulturesOfLastPage { get; private set; } = [];
    public List<DomainPresentationModel> LastCreatedDomains { get; private set; } = [];
    public List<Guid?> SavedPages { get; } = [];
    public List<Guid> PublishedPages { get; } = [];

    public Task ClearDocumentsAsync(CancellationToken cancellationToken)
    {
        Operations.Add("clear");

        return Task.CompletedTask;
    }

    public Task<ICollection<DocumentUrlInfoResponseModel>> GetDocumentUrlsAsync(IEnumerable<Guid>? id, CancellationToken cancellationToken)
    {
        return Task.FromResult<ICollection<DocumentUrlInfoResponseModel>>([]);
    }

    public Task PostDocumentAsync(CreateDocumentRequestModel? body, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(body);
        CreatedDocuments.Add(body);
        Operations.Add("save");
        SavedPages.Add(body.Id);

        return Task.CompletedTask;
    }

    public Task PutDocumentByIdDomainsAsync(Guid id, UpdateDomainsRequestModel? body, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(body);
        LastCreatedDomains = [.. body.Domains];
        Operations.Add("save domains");

        return Task.CompletedTask;
    }

    public Task PutDocumentByIdPublishAsync(Guid id, PublishDocumentRequestModel? body, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(body);

        PublishedCulturesOfLastPage = [.. body.PublishSchedules.Select(item => item.Culture)];
        Operations.Add("publish");
        PublishedPages.Add(id);

        return Task.CompletedTask;
    }
}