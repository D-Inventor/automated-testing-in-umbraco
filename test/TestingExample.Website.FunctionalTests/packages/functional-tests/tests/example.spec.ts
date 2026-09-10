import { test, expect } from '@playwright/test';
import { ApiScenario } from 'scenario-builder';

test('navigates to the home page using a Scenario', async ({ page }) => {
  const scenario = new ApiScenario();

  await page.goto('https://playwright.dev');

  await expect(page).toHaveTitle(/Playwright/);
});
