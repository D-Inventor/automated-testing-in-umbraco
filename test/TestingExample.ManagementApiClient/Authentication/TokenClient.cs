using Duende.IdentityModel.Client;

namespace TestingExample.ManagementApiClient.Authentication;

public class TokenClient(HttpClient httpClient, TokenConfiguration configuration)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly TokenConfiguration _configuration = configuration;

    public Task<TokenResponse> GetClientCredentialsAsync(CancellationToken cancellationToken)
    => _httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
    {
        Address = "/umbraco/management/api/v1/security/back-office/token",
        ClientId = _configuration.ClientID,
        ClientSecret = _configuration.ClientSecret
    }, cancellationToken);
}
