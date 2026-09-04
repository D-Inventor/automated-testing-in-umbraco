import { describe, it, expect, vi } from 'vitest';
import { ApiScenario } from '@/scenario/scenario';
import type { ContentItem } from '@/scenario/content-item';
import { postDocument, putDocumentByIdDomains } from '@/client';
import { cultureVariant } from '@/domain/variation';
import { ContentPage, type Scenario } from '@/domain/content-page';

vi.mock('@/client', () => ({
  postDocument: vi.fn().mockResolvedValue(undefined),
  putDocumentByIdDomains: vi.fn().mockResolvedValue(undefined),
}));

class TestContentType extends ContentPage {
  public static contenttype = 'fd4a241b-f3fe-4870-81cd-58fa96f029b9';

  constructor(scenario: Scenario) {
    super(scenario, TestContentType.contenttype);
  }
}

describe('Scenario', () => {
  it('should create new content items', async () => {
    // given
    const scenario = new ApiScenario();

    // when
    scenario.add({
      id: 'e5de3c64-30bb-47e5-9705-43b078515c4f',
      parent: '7c0aa964-8deb-4c09-acfe-c5a4b56a498b',
      documentType: 'a654b58b-3abf-4162-a21f-34892ba27508',
      template: 'cae04b35-6d2f-4e3f-a015-a453772bffbc',
      values: [
        {
          variation: cultureVariant('nl'),
          alias: 'contentAlias',
          value: 23,
        },
      ],
      variants: [
        {
          variation: cultureVariant('nl'),
          name: 'Example content',
        },
      ],
      domains: [],
      published: [],
      level: 0,
    });
    await scenario.build();

    // then
    expect(postDocument).toHaveBeenCalledWith({
      body: {
        id: 'e5de3c64-30bb-47e5-9705-43b078515c4f',
        parent: { id: '7c0aa964-8deb-4c09-acfe-c5a4b56a498b' },
        documentType: { id: 'a654b58b-3abf-4162-a21f-34892ba27508' },
        template: { id: 'cae04b35-6d2f-4e3f-a015-a453772bffbc' },
        values: [
          {
            culture: 'nl',
            segment: null,
            alias: 'contentAlias',
            value: 23,
          },
        ],
        variants: [
          {
            culture: 'nl',
            segment: null,
            name: 'Example content',
          },
        ],
      },
    });
  });

  it('should create new domains', async () => {
    // given
    const scenario = new ApiScenario();
    const contentItem = createMinimalContentItem('c9a7115f-11c7-410f-98eb-a48f0da125cb');
    contentItem.domains = [
      {
        culture: 'nl',
        url: 'https://localhost:44384/',
      },
    ];
    scenario.add(contentItem);

    // when
    await scenario.build();

    // then
    expect(putDocumentByIdDomains).toHaveBeenCalledWith({
      path: {
        id: 'c9a7115f-11c7-410f-98eb-a48f0da125cb',
      },
      body: {
        domains: [
          {
            domainName: 'https://localhost:44384/',
            isoCode: 'nl',
          },
        ],
      },
    });
  });

  it('should create content items in order of level', async () => {
    // given
    const apiScenario = new ApiScenario();
    const child = new TestContentType(apiScenario);
    const parent = new TestContentType(apiScenario);
    const grandparent = new TestContentType(apiScenario);

    parent.hasParent(grandparent);
    child.hasParent(parent);

    // when
    vi.clearAllMocks();
    await apiScenario.build();

    // then
    const callOrder = vi.mocked(postDocument).mock.calls;
    expect(callOrder).toHaveLength(3);
    expect(callOrder[0]![0]!.body!.id).toBe(grandparent.id); // level 0 created first
    expect(callOrder[1]![0]!.body!.id).toBe(parent.id); // level 1 created second
    expect(callOrder[2]![0]!.body!.id).toBe(child.id); // level 2 created third
  });

  it('should sort items by level', async () => {
    // given
    const apiScenario = new ApiScenario();
    const level0Item = new TestContentType(apiScenario);
    const level1Item = new TestContentType(apiScenario);
    level1Item.hasParent(level0Item);

    // when
    vi.clearAllMocks();
    await apiScenario.build();

    // then
    const callOrder = vi.mocked(postDocument).mock.calls;
    expect(callOrder).toHaveLength(2);
    expect(callOrder[0]![0]!.body!.id).toBe(level0Item.id);
    expect(callOrder[1]![0]!.body!.id).toBe(level1Item.id);
  });

  it('should sort items by order when at the same level', async () => {
    // given
    const apiScenario = new ApiScenario();
    const parent = new TestContentType(apiScenario);

    const firstChild = new TestContentType(apiScenario);
    firstChild.hasParent(parent);
    firstChild.hasOrder(2);

    const secondChild = new TestContentType(apiScenario);
    secondChild.hasParent(parent);
    secondChild.hasOrder(1);

    // when
    vi.clearAllMocks();
    await apiScenario.build();

    // then
    const callOrder = vi.mocked(postDocument).mock.calls;
    expect(callOrder).toHaveLength(3);
    expect(callOrder[0]![0]!.body!.id).toBe(parent.id); // level 0
    expect(callOrder[1]![0]!.body!.id).toBe(secondChild.id); // level 1, order 1
    expect(callOrder[2]![0]!.body!.id).toBe(firstChild.id); // level 1, order 2
  });

  it('should prioritize level over order in sorting', async () => {
    // given
    const apiScenario = new ApiScenario();
    const grandparent = new TestContentType(apiScenario);

    const parent = new TestContentType(apiScenario);
    parent.hasParent(grandparent);
    parent.hasOrder(1);

    const child = new TestContentType(apiScenario);
    child.hasParent(parent);
    child.hasOrder(99); // Higher order than parent

    // when
    vi.clearAllMocks();
    await apiScenario.build();

    // then
    const callOrder = vi.mocked(postDocument).mock.calls;
    expect(callOrder).toHaveLength(3);
    expect(callOrder[0]![0]!.body!.id).toBe(grandparent.id); // level 0
    expect(callOrder[1]![0]!.body!.id).toBe(parent.id); // level 1, order 1
    expect(callOrder[2]![0]!.body!.id).toBe(child.id); // level 2, order 99
  });
});

function createMinimalContentItem(id: string): ContentItem {
  return {
    id: id,
    documentType: 'fc6c106e-3453-43ae-b77d-4ab748d650dc',
    values: [],
    variants: [],
    domains: [],
    published: [],
    level: 0,
    order: 0,
  };
}
