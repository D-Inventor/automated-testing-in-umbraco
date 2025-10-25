using System.Diagnostics.CodeAnalysis;

using Duende.IdentityModel.Client;

namespace TestingExample.ManagementApiClient.Authentication;

public class TokenManager(TokenClient client, TimeProvider clock)
{
    private readonly TokenClient _client = client;
    private readonly TimeProvider _clock = clock;

    private TokenResponse? _latestToken = null;
    private DateTimeOffset _requestedOn;

    public TokenManager(TokenClient client)
        : this(client, TimeProvider.System)
    {
    }

    public ValueTask AuthenticateAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (TryGetAccessToken(out var accessToken))
        {
            request.SetBearerToken(accessToken);
            return ValueTask.CompletedTask;
        }

        return RefreshAndAuthenticateAsync(request, cancellationToken);
    }

    private async ValueTask RefreshAndAuthenticateAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var accessToken = await RefreshAsync(cancellationToken);
        request.SetBearerToken(accessToken);
    }

    private async Task<string> RefreshAsync(CancellationToken cancellationToken)
    {
        _latestToken = await _client.GetClientCredentialsAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(_latestToken.AccessToken)) throw new InvalidOperationException("Failed to obtain an access token");
        _requestedOn = _clock.GetUtcNow();

        return _latestToken.AccessToken;
    }

    private bool TryGetAccessToken([NotNullWhen(true)] out string? accessToken)
    {
        (var result, accessToken) = _latestToken?.AccessToken is not null && _requestedOn.AddSeconds(_latestToken.ExpiresIn) > _clock.GetUtcNow()
            ? (true, _latestToken.AccessToken)
            : (false, null);
        return result;
    }
}
