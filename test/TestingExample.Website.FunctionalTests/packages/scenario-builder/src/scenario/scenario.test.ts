import { describe, it, expect, vi } from 'vitest';
import { ApiScenario } from '@/scenario/scenario';
import type { ContentItem } from '@/scenario/content-item';
import { postDocument, putDocumentByIdDomains } from '@/client';
import { cultureVariant } from '@/domain/variation';

vi.mock('@/client', () => ({
  postDocument: vi.fn().mockResolvedValue(undefined),
  putDocumentByIdDomains: vi.fn().mockResolvedValue(undefined),
}));

describe('Scenario', () => {
  it('creates new content items', async () => {
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

  it('creates new domains', async () => {
    // given
    const scenario = new ApiScenario();
    scenario.add(createMinimalContentItem('c9a7115f-11c7-410f-98eb-a48f0da125cb'));

    // when
    scenario.addDomain({
      culture: 'nl',
      url: 'https://localhost:44384/',
      content: 'c9a7115f-11c7-410f-98eb-a48f0da125cb',
    });
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
});

function createMinimalContentItem(id: string): ContentItem {
  return {
    id: id,
    documentType: 'fc6c106e-3453-43ae-b77d-4ab748d650dc',
    values: [],
    variants: [],
  };
}
