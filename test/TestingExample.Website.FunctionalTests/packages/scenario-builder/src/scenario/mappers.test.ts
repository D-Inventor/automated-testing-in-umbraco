import { describe, it, expect } from 'vitest';
import { convertToContentPostRequest } from './mappers';
import type { ContentItem } from './content-item';
import { cultureVariant } from '../domain/variation';

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

describe('Content Model Mappers', () => {
  it('should map content item to post request', () => {
    // given
    const contentId = 'e5de3c64-30bb-47e5-9705-43b078515c4f';
    const contentItem = createMinimalContentItem({
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
    });

    // when
    const result = convertToContentPostRequest(contentItem);

    // then
    expect(result).toEqual({
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

  it('should map content item without optional fields', () => {
    // given
    const contentItem = createMinimalContentItem({
      documentType: 'a654b58b-3abf-4162-a21f-34892ba27508',
    });

    // when
    const result = convertToContentPostRequest(contentItem);

    // then
    expect(result.parent).toBeNull();
    expect(result.template).toBeNull();
    expect(result.documentType).toEqual({ id: 'a654b58b-3abf-4162-a21f-34892ba27508' });
  });
});
