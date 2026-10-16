import { beforeEach, describe, expect, it, vi } from 'vitest';
import { ScenarioClient } from './fixtures/scenario-client';
import { ApiScenario } from './scenario';
import { ContentPage, type Scenario } from '../domain/content-page';

vi.mock('./scenario-client');

const scenarioClient = new ScenarioClient();

beforeEach(() => {
  scenarioClient.reset();
});

describe('Scenario', () => {
  it('should create content items in order of level', async () => {
    // given
    const apiScenario = new ApiScenario();
    const grandparent = createTestContentType(apiScenario);
    const parent = createTestContentType(apiScenario, { parent: grandparent });
    const child = createTestContentType(apiScenario, { parent });

    // when
    await apiScenario.build();

    // then
    expect(scenarioClient.createdItemIds).toEqual([grandparent.id, parent.id, child.id]);
  });

  it('should sort items by level', async () => {
    // given
    const apiScenario = new ApiScenario();
    const level0Item = createTestContentType(apiScenario);
    const level1Item = createTestContentType(apiScenario, { parent: level0Item });

    // when
    await apiScenario.build();

    // then
    expect(scenarioClient.createdItemIds).toEqual([level0Item.id, level1Item.id]);
  });

  it('should sort items by order when at the same level', async () => {
    // given
    const apiScenario = new ApiScenario();
    const parent = createTestContentType(apiScenario);
    const firstChild = createTestContentType(apiScenario, { parent, order: 2 });
    const secondChild = createTestContentType(apiScenario, { parent, order: 1 });

    // when
    await apiScenario.build();

    // then
    expect(scenarioClient.createdItemIds).toEqual([parent.id, secondChild.id, firstChild.id]);
  });

  it('should prioritize level over order in sorting', async () => {
    // given
    const apiScenario = new ApiScenario();
    const grandparent = createTestContentType(apiScenario);
    const parent = createTestContentType(apiScenario, { parent: grandparent, order: 99 });
    const child = createTestContentType(apiScenario, { parent, order: 1 });

    // when
    await apiScenario.build();

    // then
    expect(scenarioClient.createdItemIds).toEqual([grandparent.id, parent.id, child.id]);
  });

  it('should call operations in correct order', async () => {
    // given
    const apiScenario = new ApiScenario();
    createTestContentType(apiScenario);

    // when
    await apiScenario.build();

    // then
    expect(scenarioClient.operationOrder).toEqual(['clean', 'create', 'publish', 'backfill']);
  });
});

class TestContentType extends ContentPage {
  public static contenttype = 'fd4a241b-f3fe-4870-81cd-58fa96f029b9';

  constructor(scenario: Scenario) {
    super(scenario, TestContentType.contenttype);
  }
}

interface TestContentTypeOptions {
  parent?: TestContentType;
  order?: number;
}

function createTestContentType(
  scenario: Scenario,
  options?: TestContentTypeOptions,
): TestContentType {
  const item = new TestContentType(scenario);
  if (options?.parent) {
    item.hasParent(options.parent);
  }
  if (options?.order !== undefined) {
    item.hasOrder(options.order);
  }
  return item;
}
