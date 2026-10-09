import { describe, it, expect } from 'vitest';
import { ApiScenario } from './scenario';
import type { ContentItem } from './content-item';
import { cultureVariant } from '../domain/variation';
import { ContentPage, type Scenario } from '../domain/content-page';
import { afterAll, beforeAll, beforeEach } from 'vitest';
import { client } from '../client/client.gen';
import { UmbracoClient } from './fixtures/umbraco-client';

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

class TestContentType extends ContentPage {
  public static contenttype = 'fd4a241b-f3fe-4870-81cd-58fa96f029b9';

  constructor(scenario: Scenario) {
    super(scenario, TestContentType.contenttype);
  }
}

describe('Scenario', () => {
  it('should create new content items', async () => {
    // given
    const contentId = 'e5de3c64-30bb-47e5-9705-43b078515c4f';
    const scenario = new ApiScenario();

    // when
    scenario.add({
      id: contentId,
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
      order: 0,
    });
    await scenario.build();

    // then
    expect(umbracoClient.lastPostDocument).toEqual({
      id: contentId,
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
    });
  });

  it('should create new domains', async () => {
    // given
    const contentId = 'c9a7115f-11c7-410f-98eb-a48f0da125cb';
    const scenario = new ApiScenario();
    scenario.add(
      createMinimalContentItem({
        id: contentId,
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
    const child = new TestContentType(apiScenario);
    const parent = new TestContentType(apiScenario);
    const grandparent = new TestContentType(apiScenario);

    parent.hasParent(grandparent);
    child.hasParent(parent);

    // when
    await apiScenario.build();

    // then
    expect(umbracoClient.postedDocumentIds).toHaveLength(3);
    expect(umbracoClient.postedDocumentIds[0]).toBe(grandparent.id); // level 0 created first
    expect(umbracoClient.postedDocumentIds[1]).toBe(parent.id); // level 1 created second
    expect(umbracoClient.postedDocumentIds[2]).toBe(child.id); // level 2 created third
  });

  it('should sort items by level', async () => {
    // given
    const apiScenario = new ApiScenario();
    const level0Item = new TestContentType(apiScenario);
    const level1Item = new TestContentType(apiScenario);
    level1Item.hasParent(level0Item);

    // when
    await apiScenario.build();

    // then
    expect(umbracoClient.postedDocumentIds).toHaveLength(2);
    expect(umbracoClient.postedDocumentIds[0]).toBe(level0Item.id);
    expect(umbracoClient.postedDocumentIds[1]).toBe(level1Item.id);
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
    await apiScenario.build();

    // then
    expect(umbracoClient.postedDocumentIds).toHaveLength(3);
    expect(umbracoClient.postedDocumentIds[0]).toBe(parent.id); // level 0
    expect(umbracoClient.postedDocumentIds[1]).toBe(secondChild.id); // level 1, order 1
    expect(umbracoClient.postedDocumentIds[2]).toBe(firstChild.id); // level 1, order 2
  });

  it('should prioritize level over order in sorting', async () => {
    // given
    const apiScenario = new ApiScenario();
    const grandparent = new TestContentType(apiScenario);

    const parent = new TestContentType(apiScenario);
    parent.hasParent(grandparent);
    parent.hasOrder(99);

    const child = new TestContentType(apiScenario);
    child.hasParent(parent);
    child.hasOrder(1); // Higher order than parent

    // when
    await apiScenario.build();

    // then
    expect(umbracoClient.postedDocumentIds).toHaveLength(3);
    expect(umbracoClient.postedDocumentIds[0]).toBe(grandparent.id); // level 0
    expect(umbracoClient.postedDocumentIds[1]).toBe(parent.id); // level 1, order 99
    expect(umbracoClient.postedDocumentIds[2]).toBe(child.id); // level 2, order 1
  });

  it('should throw when postDocument fails', async () => {
    // given
    umbracoClient.postDocumentResponse(
      new Response(JSON.stringify({ message: 'Failed to post the document' }), {
        status: 500,
      }),
    );

    const apiScenario = new ApiScenario();
    apiScenario.add(createMinimalContentItem({ id: 'e5de3c64-30bb-47e5-9705-43b078515c4f' }));

    // when & then
    await expect(apiScenario.build()).rejects.toThrow();
  });

  it('should throw when putDocumentByIdDomains fails', async () => {
    // given
    umbracoClient.putDomainsResponse(
      new Response(JSON.stringify({ message: 'Failed to configure domains' }), {
        status: 500,
      }),
    );

    const apiScenario = new ApiScenario();
    apiScenario.add(
      createMinimalContentItem({ domains: [{ culture: 'en', url: 'https://example.com' }] }),
    );

    // when & then
    await expect(apiScenario.build()).rejects.toThrow();
  });
});

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
