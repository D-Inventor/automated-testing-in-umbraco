import test, { expect } from '@playwright/test';
import { basicScenario, EnglishCulture } from '@scenario/basic-scenario';
import { Invariant } from 'scenario-builder';

test.describe('homepage', () => {
  test('should display title from content', async ({ page }) => {
    // given
    let { content, scenario } = basicScenario();
    content.homepage.hasHeader(Invariant, { title: 'welcome to the website' });
    await scenario.build();

    // when
    // await page.goto(urlFor(scenario.website, EnglishCulture));

    // then
    await expect(page).toHaveTitle('welcome to the website');
  });
});
