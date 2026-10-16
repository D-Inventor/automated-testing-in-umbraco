import test, { expect } from '@playwright/test';
import { basicScenario } from '@scenario/basic-scenario';
import { Invariant } from 'scenario-builder';

test.describe('homepage', () => {
  test('should display title from content', async ({ page }) => {
    // given
    const { content, scenario } = basicScenario();
    content.homepage.hasHeader(Invariant, { title: 'welcome to the website' });
    await scenario.build();

    // when
    await page.goto(content.website.url('en-US').toString());

    // then
    await expect(page.locator('h1').innerText()).resolves.toBe('welcome to the website');
  });
});
