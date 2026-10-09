import {
  postDocument,
  putDocumentByIdDomains,
  type CreateDocumentRequestModel,
  type DocumentValueModel,
  type DocumentVariantRequestModel,
  type ReferenceByIdModel,
} from '../client';
import type { ContentItem, ContentItemValue, ContentItemVariant } from './content-item';

class ApiError extends Error {
  constructor(
    public response: Response | undefined,
    error: unknown,
  ) {
    super(
      `Response failed [${response?.status}: ${response?.statusText}]\n${JSON.stringify(error, null, 2)}`,
    );
    this.name = 'ApiError';
  }
}

export interface Scenario {
  add(content: ContentItem): void;

  build(): Promise<void>;
}

export class ApiScenario implements Scenario {
  private added: Record<string, ContentItem> = {};

  public add(content: ContentItem) {
    this.added[content.id] = content;
  }

  public async build(): Promise<void> {
    const sortedItems = Object.values(this.added).sort(
      (a, b) => (a.level ?? 0) - (b.level ?? 0) || (a.order ?? 0) - (b.order ?? 0),
    );

    for (const item of sortedItems) {
      const { error, response } = await postDocument({
        body: convertToContentPostRequest(item),
      });

      if (error) {
        throw new ApiError(response, error);
      }

      if (item.domains && item.domains.length > 0) {
        const { error, response } = await putDocumentByIdDomains({
          path: {
            id: item.id,
          },
          body: {
            domains: item.domains.map((domain) => ({
              domainName: domain.url,
              isoCode: domain.culture,
            })),
          },
        });

        if (error) {
          throw new ApiError(response, error);
        }
      }
    }
  }
}

function convertToContentPostRequest(item: ContentItem): CreateDocumentRequestModel {
  return {
    id: item.id,
    parent: item.parent !== undefined ? convertToReference(item.parent) : null,
    documentType: convertToReference(item.documentType),
    template: item.template !== undefined ? convertToReference(item.template) : null,
    values: item.values.map(convertToRequestValue),
    variants: item.variants.map(convertToRequestVariant),
  };
}

function convertToReference(id: string): ReferenceByIdModel {
  return { id: id };
}

function convertToRequestValue(value: ContentItemValue): DocumentValueModel {
  return {
    alias: value.alias,
    culture: value.variation.culture,
    segment: value.variation.segment,
    value: value.value,
  };
}

function convertToRequestVariant(variant: ContentItemVariant): DocumentVariantRequestModel {
  return {
    culture: variant.variation.culture,
    segment: variant.variation.segment,
    name: variant.name,
  };
}
