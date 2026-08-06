import { test, expect } from '@playwright/test';
import { Scenario } from 'scenario-builder';

test('navigates to the home page using a Scenario', async ({ page }) => {
  const scenario = new Scenario({ baseUrl: 'https://playwright.dev', path: '/' });

  await page.goto(scenario.url);

  await expect(page).toHaveTitle(/Playwright/);
});
