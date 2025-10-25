using System.Text;

using TestingExample.ManagementApiClient.Authentication;

namespace TestingExample.ManagementApiClient;

public class ManagementApiClientBase
{
    private readonly TokenManager _tokenManager;

    protected ManagementApiClientBase(TokenManager tokenManager)
    {
        _tokenManager = tokenManager;
    }

    protected ValueTask PrepareRequestAsync(HttpClient httpClient, HttpRequestMessage request, StringBuilder urlBuilder, CancellationToken cancellationToken)
        => ValueTask.CompletedTask;

    protected ValueTask PrepareRequestAsync(HttpClient httpClient, HttpRequestMessage request, string url, CancellationToken cancellationToken)
        => _tokenManager.AuthenticateAsync(request, cancellationToken);

    protected ValueTask ProcessResponseAsync(HttpClient httpClient, HttpResponseMessage response, CancellationToken cancellationToken)
        => ValueTask.CompletedTask;
}
