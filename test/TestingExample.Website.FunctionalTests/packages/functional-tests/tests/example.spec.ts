import { test, expect } from '@playwright/test';

test('navigates to the home page using a Scenario', async ({ page }) => {
  await page.goto('https://playwright.dev');

  await expect(page).toHaveTitle(/Playwright/);
});
