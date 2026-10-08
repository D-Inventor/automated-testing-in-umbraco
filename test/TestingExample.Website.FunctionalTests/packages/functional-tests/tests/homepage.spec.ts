import test, { expect } from '@playwright/test';
import { basicScenario, English } from '@scenario/basic-scenario';

test.describe('homepage', () => {
  test('should display title from content', async ({ page }) => {
    // given
    const { content, scenario } = basicScenario();
    content.homepage.hasHeader(English, { title: 'welcome to the website' });
    try {
      await scenario.build();
    } catch (error) {
      const stack = error.stack;
      console.log(error);
    }

    // when
    await page.goto('https://localhost:44356/');

    // then
    await expect(page).toHaveTitle('welcome to the website');
  });
});
