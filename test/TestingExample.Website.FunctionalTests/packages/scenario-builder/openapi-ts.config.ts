import { defineConfig } from '@hey-api/openapi-ts';

export default defineConfig({
  input: 'https://localhost:44376/umbraco/openapi/management.json',
  output: 'src/client',
  plugins: [
    {
      name: '@hey-api/client-fetch',
      throwOnError: false,
      runtimeConfigPath: './src/client-config',
    },
    '@hey-api/sdk',
    '@hey-api/schemas',
    'msw',
  ],
});
