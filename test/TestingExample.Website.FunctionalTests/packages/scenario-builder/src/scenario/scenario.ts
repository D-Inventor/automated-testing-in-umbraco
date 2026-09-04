import {
  postDocument,
  putDocumentByIdDomains,
  type CreateDocumentRequestModel,
  type DocumentValueModel,
  type DocumentVariantRequestModel,
  type ReferenceByIdModel,
} from '@/client';
import type { ContentItem, ContentItemValue, ContentItemVariant } from '@/scenario/content-item';

export interface Scenario {
  add(content: ContentItem): void;
}

export class ApiScenario implements Scenario {
  private added: Record<string, ContentItem> = {};

  public add(content: ContentItem) {
    this.added[content.id] = content;
  }

  public async build(): Promise<void> {
    for (const item of Object.values(this.added)) {
      await postDocument({
        body: convertToContentPostRequest(item),
      });

      if (item.domains && item.domains.length > 0) {
        await putDocumentByIdDomains({
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
