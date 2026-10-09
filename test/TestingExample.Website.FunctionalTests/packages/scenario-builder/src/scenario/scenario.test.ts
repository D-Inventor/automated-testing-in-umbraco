import { describe, it, expect } from 'vitest';
import { ApiScenario } from './scenario';
import type { ContentItem } from './content-item';
import { cultureVariant } from '../domain/variation';
import { ContentPage, type Scenario } from '../domain/content-page';
import { afterAll, afterEach, beforeAll, vi } from 'vitest';
import { setupServer } from 'msw/node';
import { createMswHandlers } from '../client/msw.gen';
import { client } from '../client/client.gen';

const BASE_URL = 'https://localhost:3000';
const handlers = createMswHandlers({ baseUrl: BASE_URL });
const server = setupServer();

beforeAll(() => {
  client.setConfig({ baseUrl: BASE_URL });
  server.listen({ onUnhandledFrame: 'error' });
});

afterEach(() => {
  server.resetHandlers();
  vi.clearAllMocks();
});

afterAll(() => {
  server.close();
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
    let capturedBody: unknown;
    server.use(
      handlers.pick.postDocument(({ request }) => {
        return request.json().then((body) => {
          capturedBody = body;
          return new Response(undefined, { status: 201 });
        });
      }),
    );

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
    expect(capturedBody).toEqual({
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
    let capturedDomainBody: unknown;
    server.use(
      handlers.pick.postDocument(() => {
        return new Response(undefined, { status: 201 });
      }),
      handlers.pick.putDocumentByIdDomains(({ request }) => {
        return request.json().then((body) => {
          capturedDomainBody = body;
          return new Response(undefined, { status: 200 });
        });
      }),
    );

    const scenario = new ApiScenario();
    const contentItem = createMinimalContentItem({ id: contentId });
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
    expect(capturedDomainBody).toEqual({
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
    const callOrder: (string | null | undefined)[] = [];
    server.use(
      handlers.pick.postDocument(({ request }) => {
        return request.json().then((body) => {
          callOrder.push(body.id);
          return new Response(undefined, { status: 201 });
        });
      }),
    );

    const apiScenario = new ApiScenario();
    const child = new TestContentType(apiScenario);
    const parent = new TestContentType(apiScenario);
    const grandparent = new TestContentType(apiScenario);

    parent.hasParent(grandparent);
    child.hasParent(parent);

    // when
    await apiScenario.build();

    // then
    expect(callOrder).toHaveLength(3);
    expect(callOrder[0]).toBe(grandparent.id); // level 0 created first
    expect(callOrder[1]).toBe(parent.id); // level 1 created second
    expect(callOrder[2]).toBe(child.id); // level 2 created third
  });

  it('should sort items by level', async () => {
    // given
    const callOrder: (string | null | undefined)[] = [];
    server.use(
      handlers.pick.postDocument(({ request }) => {
        return request.json().then((body) => {
          callOrder.push(body.id);
          return new Response(undefined, { status: 201 });
        });
      }),
    );

    const apiScenario = new ApiScenario();
    const level0Item = new TestContentType(apiScenario);
    const level1Item = new TestContentType(apiScenario);
    level1Item.hasParent(level0Item);

    // when
    await apiScenario.build();

    // then
    expect(callOrder).toHaveLength(2);
    expect(callOrder[0]).toBe(level0Item.id);
    expect(callOrder[1]).toBe(level1Item.id);
  });

  it('should sort items by order when at the same level', async () => {
    // given
    const callOrder: (string | null | undefined)[] = [];
    server.use(
      handlers.pick.postDocument(({ request }) => {
        return request.json().then((body) => {
          callOrder.push(body.id);
          return new Response(undefined, { status: 201 });
        });
      }),
    );

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
    expect(callOrder).toHaveLength(3);
    expect(callOrder[0]).toBe(parent.id); // level 0
    expect(callOrder[1]).toBe(secondChild.id); // level 1, order 1
    expect(callOrder[2]).toBe(firstChild.id); // level 1, order 2
  });

  it('should prioritize level over order in sorting', async () => {
    // given
    const callOrder: (string | null | undefined)[] = [];
    server.use(
      handlers.pick.postDocument(({ request }) => {
        return request.json().then((body) => {
          callOrder.push(body.id);
          return new Response(undefined, { status: 201 });
        });
      }),
    );

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
    expect(callOrder).toHaveLength(3);
    expect(callOrder[0]).toBe(grandparent.id); // level 0
    expect(callOrder[1]).toBe(parent.id); // level 1, order 99
    expect(callOrder[2]).toBe(child.id); // level 2, order 1
  });

  it('should throw when postDocument fails', async () => {
    // given
    const errorMessage = 'Failed to post the document';
    server.use(
      handlers.pick.postDocument(() => {
        return new Response(JSON.stringify({ message: errorMessage }), {
          status: 500,
        });
      }),
    );

    const apiScenario = new ApiScenario();
    apiScenario.add(createMinimalContentItem({ id: 'e5de3c64-30bb-47e5-9705-43b078515c4f' }));

    // when & then
    await expect(apiScenario.build()).rejects.toThrow();
  });

  it('should throw when putDocumentByIdDomains fails', async () => {
    // given
    const errorMessage = 'Failed to configure domains';
    server.use(
      handlers.pick.postDocument({
        body: undefined,
        status: 201,
      }),
      handlers.pick.putDocumentByIdDomains(() => {
        return new Response(JSON.stringify({ message: errorMessage }), {
          status: 500,
        });
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
