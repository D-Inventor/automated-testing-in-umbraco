import { describe, expect, it } from 'vitest';
import { ContentPage, type Scenario } from './content-page';
import type { ContentItem } from '@/scenario/content-item';
import { cultureVariant } from './variation';

class TestContentType extends ContentPage {
  public static contenttype = 'fd4a241b-f3fe-4870-81cd-58fa96f029b9';

  constructor(scenario: Scenario) {
    super(scenario, TestContentType.contenttype);
  }
}

function looksLikeAGuid(value: unknown): boolean {
  if (typeof value !== 'string') return false;
  const guidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;
  return guidRegex.test(value);
}

class SpyScenario implements Scenario {
  public registered_content: ContentItem[] = [];

  add(content: ContentItem): void {
    this.registered_content.push(content);
  }
}

describe('ContentPage', () => {
  it('should register content item in scenario', () => {
    // given
    const scenario = new SpyScenario();

    // when
    new TestContentType(scenario);

    // then
    expect(scenario.registered_content).toHaveLength(1);
    expect(scenario.registered_content[0]!.id).toSatisfy((id) => looksLikeAGuid(id));
    expect(scenario.registered_content[0]!.documentType).toBe(TestContentType.contenttype);
  });

  it('should assign parent when hasParent is called', () => {
    // given
    const scenario = new SpyScenario();
    const parent = new TestContentType(scenario);
    const child = new TestContentType(scenario);

    // when
    child.hasParent(parent);

    // then
    const childItem = scenario.registered_content.find((item) => item.id === child.id);
    expect(childItem!.parent).toBe(parent.id);
  });

  it('should add domain when hasDomain is called', () => {
    // given
    const scenario = new SpyScenario();
    const content = new TestContentType(scenario);
    const culture = 'en';
    const url = new URL('https://example.com');

    // when
    content.hasDomain(culture, url);

    // then
    const contentItem = scenario.registered_content.find((item) => item.id === content.id);
    expect(contentItem!.domains).toHaveLength(1);
    expect(contentItem!.domains![0]).toEqual({ culture, url: url.toString() });
  });

  it('should add variant when hasVariation is called', () => {
    // given
    const scenario = new SpyScenario();
    const content = new TestContentType(scenario);
    const variation = cultureVariant('en');
    const name = 'English variant';

    // when
    content.hasVariation(variation, name);

    // then
    const contentItem = scenario.registered_content.find((item) => item.id === content.id);
    expect(contentItem!.variants).toHaveLength(1);
    expect(contentItem!.variants[0]).toEqual({ variation, name });
  });

  it('should mark variation as published when isPublishedIn is called', () => {
    // given
    const scenario = new SpyScenario();
    const content = new TestContentType(scenario);
    const variation = cultureVariant('en');
    const name = 'English variant';

    // when
    content.hasVariation(variation, name);
    content.isPublishedIn(variation);

    // then
    const contentItem = scenario.registered_content.find((item) => item.id === content.id);
    expect(contentItem!.published).toHaveLength(1);
    expect(contentItem!.published![0]).toEqual(variation);
  });

  it('should throw error when publishing a variation that has not been added', () => {
    // given
    const scenario = new SpyScenario();
    const content = new TestContentType(scenario);
    const variation = cultureVariant('en');

    // when & then
    expect(() => content.isPublishedIn(variation)).toThrow();
  });

  it('should have level 0 by default', () => {
    // given
    const scenario = new SpyScenario();
    const content = new TestContentType(scenario);

    // then
    expect(content.level).toBe(0);
  });

  it('should calculate level based on parent', () => {
    // given
    const scenario = new SpyScenario();
    const parent = new TestContentType(scenario);
    const child = new TestContentType(scenario);

    // when
    child.hasParent(parent);

    // then
    expect(child.level).toBe(1);
  });

  it('should update child level when parent level changes', () => {
    // given
    const scenario = new SpyScenario();
    const grandparent = new TestContentType(scenario);
    const parent = new TestContentType(scenario);
    const child = new TestContentType(scenario);

    parent.hasParent(grandparent);
    child.hasParent(parent);

    // when
    grandparent.hasParent(new TestContentType(scenario));

    // then
    expect(parent.level).toBe(2);
    expect(child.level).toBe(3);
  });

  it('should update level when changing parent', () => {
    // given
    const scenario = new SpyScenario();
    const parent1 = new TestContentType(scenario);
    const parent2 = new TestContentType(scenario);
    parent2.hasParent(parent1);
    const child = new TestContentType(scenario);

    child.hasParent(parent1);

    // when
    child.hasParent(parent2);

    // then
    expect(parent1.level).toBe(0);
    expect(parent2.level).toBe(1);
    expect(child.level).toBe(2);
  });
});
