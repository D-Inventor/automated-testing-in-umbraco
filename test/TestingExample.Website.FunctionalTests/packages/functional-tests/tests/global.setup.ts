import { setupClient } from 'scenario-builder';
import { test as setup } from '@playwright/test';

setup('Setup connection with the server', async () => {
  // TODO: Add your setup logic here
  // Example:
  // - Start your web server
  // - Initialize database
  // - Set environment variables
  // - Seed test data

  await setupClient();

  console.log('✓ Global setup completed successfully');
});
