import { defineConfig } from '@hey-api/openapi-ts';

export default defineConfig({
  input: 'https://localhost:44376/umbraco/openapi/management.json',
  output: 'src/client',
});
