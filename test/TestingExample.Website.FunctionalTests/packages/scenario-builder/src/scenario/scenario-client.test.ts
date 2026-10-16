import { afterAll, beforeAll, beforeEach, describe, expect, it } from 'vitest';
import { UmbracoClient } from './fixtures/umbraco-client';
import { client } from '@/client/client.gen';
import type { ContentItem } from './content-item';
import { backfillContentUrls, createContentTree, publishContentTree } from './scenario-client';
import { Invariant } from '@/domain/variation';

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

describe('createContentTree', () => {
  it('should throw when postDocument fails', async () => {
    // given
    umbracoClient.postDocumentResponse(http500Response('Failed to post the document'));

    // when & then
    await expect(createContentTree([createMinimalContentItem({})])).rejects.toThrow();
  });

  it('should throw when putDocumentByIdDomains fails', async () => {
    // given
    umbracoClient.putDomainsResponse(http500Response('Failed to configure domains'));

    // when & then
    await expect(
      createContentTree([
        createMinimalContentItem({ domains: [{ culture: 'en', url: 'https://example.com' }] }),
      ]),
    ).rejects.toThrow();
  });

  it('should create new domains', async () => {
    // given / when
    await createContentTree([
      createMinimalContentItem({
        domains: [
          {
            culture: 'nl',
            url: 'https://localhost:44384/',
          },
        ],
      }),
    ]);

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
});

describe('publishContentTree', () => {
  it('should publish documents', async () => {
    // given
    const first = createMinimalContentItem({ published: [Invariant] });
    const second = createMinimalContentItem({ published: [Invariant] });
    const third = createMinimalContentItem({ published: [Invariant] });

    // when
    await publishContentTree([first, second, third]);

    // then
    expect(umbracoClient.publishedDocumentIds).toEqual([first.id, second.id, third.id]);
  });
});

describe('backfillContentUrls', () => {
  it('should populate urls for published content in invariant culture', async () => {
    // given
    const item = createMinimalContentItem({});
    umbracoClient.contentUrlsResponse(item.id, 'en', ['https://example.com/home']);

    // when
    await backfillContentUrls([item]);

    // then
    expect(item.urls).toEqual([{ culture: 'en', url: new URL('https://example.com/home') }]);
  });

  it('should make relative URLs absolute', async () => {
    // given
    const item = createMinimalContentItem({});
    umbracoClient.contentUrlsResponse(item.id, 'en', ['/home']);
    const expectedUrl = umbracoClient.absoluteUrl('/home');

    // when
    await backfillContentUrls([item]);

    // then
    expect(item.urls).toEqual([{ culture: 'en', url: expectedUrl }]);
  });
});

function http500Response(message: string): Response {
  return new Response(JSON.stringify({ message: message }), {
    status: 500,
  });
}

function createMinimalContentItem(values: Partial<ContentItem>): ContentItem {
  return {
    id: 'e5de3c64-30bb-47e5-9705-43b078515c4f',
    documentType: 'fc6c106e-3453-43ae-b77d-4ab748d650dc',
    values: [],
    variants: [],
    domains: [],
    published: [],
    urls: [],
    level: 0,
    order: 0,
    ...values,
  };
}
