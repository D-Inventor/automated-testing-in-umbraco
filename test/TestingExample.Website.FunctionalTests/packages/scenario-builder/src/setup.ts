import { client } from './client/client.gen';

export async function setupClient() {
  client.setConfig({
    baseUrl: 'https://localhost:44376/',
  });
}
