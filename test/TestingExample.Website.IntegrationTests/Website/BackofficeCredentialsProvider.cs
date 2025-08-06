using Duende.IdentityModel.Client;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core;

namespace TestingExample.Website.IntegrationTests.Website;

public sealed class BackofficeCredentialsProvider(IUserService userService, IBackOfficeUserClientCredentialsManager clientCredentialService)
{
    private readonly IUserService _userService = userService;
    private readonly IBackOfficeUserClientCredentialsManager _clientCredentialService = clientCredentialService;

    private BackofficeCredentials? _credentials = null;

    public async Task AuthenticateAsBackofficeUserAsync(HttpClient client, CancellationToken cancellationToken = default)
    {
        // If no credentials are set, create a new backoffice user and client credentials
        _credentials ??= await CreateBackofficeCredentialsAsync();

        // Use the credentials to authenticate the HttpClient
        await AuthenticateHttpClientAsync(client, _credentials, cancellationToken);
    }

    private static async Task AuthenticateHttpClientAsync(HttpClient client, BackofficeCredentials credentials, CancellationToken cancellationToken)
    {
        var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        {
            Address = "/umbraco/management/api/v1/security/back-office/token",
            ClientId = credentials.ClientId,
            ClientSecret = credentials.ClientSecret
        }, cancellationToken);

        if (string.IsNullOrWhiteSpace(tokenResponse.AccessToken)) throw new InvalidOperationException("Failed to obtain access token for backoffice user.");

        client.SetBearerToken(tokenResponse.AccessToken);
    }

    private async Task<BackofficeCredentials> CreateBackofficeCredentialsAsync()
    {
        var userCreateResult = await _userService.CreateAsync(Constants.Security.SuperUserKey, new UserCreateModel
        {
            Name = "Test User",
            UserName = "test@email.com",
            Email = "test@email.com",
            Kind = UserKind.Api,
            UserGroupKeys = new HashSet<Guid> { Constants.Security.AdminGroupKey }
        }, approveUser: true);

        var user = userCreateResult.Result.CreatedUser!;

        var result = new BackofficeCredentials("umbraco-back-office-" + Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        var clientCredentialResult = await _clientCredentialService.SaveAsync(user.Key, result.ClientId, result.ClientSecret);

        if (!clientCredentialResult.Success) throw new InvalidOperationException("Failed to create backoffice client credentials.");

        return result;
    }

    private record BackofficeCredentials(string ClientId, string ClientSecret);
}