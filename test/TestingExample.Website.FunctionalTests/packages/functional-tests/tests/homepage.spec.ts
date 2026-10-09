import test, { expect } from '@playwright/test';
import { basicScenario, English } from '@scenario/basic-scenario';

test.describe('homepage', () => {
  test('should display title from content', async ({ page }) => {
    // given
    const { content, scenario } = basicScenario();
    content.homepage.hasHeader(English, { title: 'welcome to the website' });
    await scenario.build();

    // when
    await page.goto('https://localhost:44376/');

    // then
    await expect(page).toHaveTitle('welcome to the website');
  });
});
