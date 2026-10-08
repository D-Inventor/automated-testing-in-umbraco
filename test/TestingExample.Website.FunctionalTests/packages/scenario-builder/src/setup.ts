import { client } from './client/client.gen';
import type { AuthToken } from './client/core/auth.gen';
import * as openidclient from 'openid-client';

export async function setupClient() {
  client.setConfig({
    baseUrl: process.env.WEBSITE_URL,
  });

  const token = await getAuthenticationToken();
  client.interceptors.request.use((request) => {
    request.headers.set('Authorization', `Bearer ${token}`);
    return request;
  });
}

async function getAuthenticationToken(): Promise<AuthToken> {
  const websiteUrl = process.env.WEBSITE_URL;
  const clientId = process.env.WEBSITE_CLIENTID;
  const clientSecret = process.env.WEBSITE_CLIENTSECRET;

  if (!websiteUrl || !clientId || !clientSecret) {
    throw new Error('WEBSITE_URL, WEBSITE_CLIENTID and WEBSITE_CLIENTSECRET must all be set');
  }

  // Umbraco's Management API does not support OIDC discovery, so the server metadata is built manually.
  const server: openidclient.ServerMetadata = {
    issuer: websiteUrl,
    token_endpoint: new URL(
      '/umbraco/management/api/v1/security/back-office/token',
      websiteUrl,
    ).toString(),
  };

  const config = new openidclient.Configuration(
    server,
    clientId,
    clientSecret,
    openidclient.ClientSecretBasic(clientSecret),
  );

  const tokens = await openidclient.clientCredentialsGrant(config);

  return tokens.access_token;
}
