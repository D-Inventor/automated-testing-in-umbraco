import { describe, it, expect } from 'vitest';
import { ApiScenario } from './scenario';
import type { ContentItem } from './content-item';
import { ContentPage, type Scenario } from '../domain/content-page';
import { afterAll, beforeAll, beforeEach } from 'vitest';
import { client } from '../client/client.gen';
import { UmbracoClient } from './fixtures/umbraco-client';
import { Invariant } from '../domain/variation';

const umbracoClient = new UmbracoClient();

beforeAll(() => {
  umbracoClient.initialize(client);
});

beforeEach(() => {
  umbracoClient.reset();
});

afterAll(() => {
  umbracoClient.close();
});

describe('Scenario', () => {
  it('should create new domains', async () => {
    // given
    const scenario = new ApiScenario();
    scenario.add(
      createMinimalContentItem({
        domains: [
          {
            culture: 'nl',
            url: 'https://localhost:44384/',
          },
        ],
      }),
    );

    // when
    await scenario.build();

    // then
    expect(umbracoClient.lastPutDomains).toEqual({
      domains: [
        {
          domainName: 'https://localhost:44384/',
          isoCode: 'nl',
        },
      ],
    });
  });

  it('should create content items in order of level', async () => {
    // given
    const apiScenario = new ApiScenario();
    const grandparent = createTestContentType(apiScenario);
    const parent = createTestContentType(apiScenario, { parent: grandparent });
    const child = createTestContentType(apiScenario, { parent });

    // when
    await apiScenario.build();

    // then
    expect(umbracoClient.postedDocumentIds).toEqual([grandparent.id, parent.id, child.id]);
  });

  it('should sort items by level', async () => {
    // given
    const apiScenario = new ApiScenario();
    const level0Item = createTestContentType(apiScenario);
    const level1Item = createTestContentType(apiScenario, { parent: level0Item });

    // when
    await apiScenario.build();

    // then
    expect(umbracoClient.postedDocumentIds).toEqual([level0Item.id, level1Item.id]);
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
    expect(umbracoClient.postedDocumentIds).toEqual([parent.id, secondChild.id, firstChild.id]);
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
    expect(umbracoClient.postedDocumentIds).toEqual([grandparent.id, parent.id, child.id]);
  });

  it('should throw when postDocument fails', async () => {
    // given
    umbracoClient.postDocumentResponse(http500Response('Failed to post the document'));

    const apiScenario = new ApiScenario();
    apiScenario.add(createMinimalContentItem({ id: 'e5de3c64-30bb-47e5-9705-43b078515c4f' }));

    // when & then
    await expect(apiScenario.build()).rejects.toThrow();
  });

  it('should throw when putDocumentByIdDomains fails', async () => {
    // given
    umbracoClient.putDomainsResponse(http500Response('Failed to configure domains'));

    const apiScenario = new ApiScenario();
    apiScenario.add(
      createMinimalContentItem({ domains: [{ culture: 'en', url: 'https://example.com' }] }),
    );

    // when & then
    await expect(apiScenario.build()).rejects.toThrow();
  });

  it('should publish documents in the same order as posting them', async () => {
    // given
    const apiScenario = new ApiScenario();
    const first = createTestContentType(apiScenario, { published: true });
    const second = createTestContentType(apiScenario, { published: true });
    const third = createTestContentType(apiScenario, { published: true });

    // when
    await apiScenario.build();

    // then
    expect(umbracoClient.publishedDocumentIds).toEqual([first.id, second.id, third.id]);
  });

  it('should publish content after ALL content has been posted', async () => {
    // given
    const apiScenario = new ApiScenario();
    createTestContentType(apiScenario, { published: true });
    createTestContentType(apiScenario, { published: true });
    createTestContentType(apiScenario, { published: true });

    // when
    await apiScenario.build();

    // then
    const lastPostIndex = umbracoClient.operationOrder.lastIndexOf('post');
    const firstPublishIndex = umbracoClient.operationOrder.indexOf('publish');
    
    expect(lastPostIndex).toBeLessThan(firstPublishIndex);
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
  published?: boolean;
}

function http500Response(message: string): Response {
  return new Response(JSON.stringify({ message: message }), {
    status: 500,
  });
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
  if (options?.published) {
    item.hasVariation(Invariant, 'Invariant');
    item.isPublishedIn(Invariant);
  }
  return item;
}

function createMinimalContentItem(values: Partial<ContentItem>): ContentItem {
  return {
    id: 'e5de3c64-30bb-47e5-9705-43b078515c4f',
    documentType: 'fc6c106e-3453-43ae-b77d-4ab748d650dc',
    values: [],
    variants: [],
    domains: [],
    published: [],
    level: 0,
    order: 0,
    ...values,
  };
}
