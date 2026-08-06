import test, { expect } from '@playwright/test';
import { Scenario } from 'scenario-builder';

test.describe('homepage', () => {

    let scenario = new Scenario();

    test.beforeEach(() => {
        scenario.withBasicContent();
    })


  test('should display title from content', async ({ page }) => {
    // given
    scenario.homepage()
        .hasContent(Variation.invariant, (content) => content.withHeader(title: "welcome to the website"));
    await scenario.build();

    // when
    const response = await page.goto(scenario.website().url)

    // then
    expect(page.title).toBe("welcome to the website")
  });
});
