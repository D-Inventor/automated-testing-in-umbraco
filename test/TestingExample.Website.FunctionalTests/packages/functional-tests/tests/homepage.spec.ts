import test, { expect } from '@playwright/test';
import { basicScenario } from '@scenario/basic-scenario';

test.describe('homepage', () => {
  test('should display title from content', async ({ page }) => {
    // given
    let scenario = basicScenario();
    scenario.homepage.hasHeader(Invariant, { title: 'welcome to the website' });
    await build(scenario);

    // when
    await page.goto(scenario.website.url(EnglishCulture));

    // then
    await expect(page).toHaveTitle('welcome to the website');
  });
});
