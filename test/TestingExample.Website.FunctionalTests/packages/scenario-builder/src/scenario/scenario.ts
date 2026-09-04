import {
  postDocument,
  putDocumentByIdDomains,
  type CreateDocumentRequestModel,
  type DocumentValueModel,
  type DocumentVariantRequestModel,
  type ReferenceByIdModel,
} from '@/client';
import type { ContentItem, ContentItemValue, ContentItemVariant } from '@/scenario/content-item';
import type { DomainItem } from '@/scenario/domain-item';

export interface Scenario {
  add(content: ContentItem): void;
}

export class ApiScenario implements Scenario {
  private added: Record<string, ContentItem> = {};
  private addedDomains: Record<string, DomainItem[]> = {};

  public add(content: ContentItem) {
    this.added[content.id] = content;
  }

  public addDomain(domain: DomainItem) {
    if (!(domain.content in this.addedDomains)) {
      this.addedDomains[domain.content] = [];
    }

    this.addedDomains[domain.content]!.push(domain);
  }

  public async build(): Promise<void> {
    for (const item of Object.values(this.added)) {
      await postDocument({
        body: convertToContentPostRequest(item),
      });
    }

    for (const item of Object.keys(this.addedDomains)) {
      await putDocumentByIdDomains({
        path: {
          id: item,
        },
        body: {
          domains: this.addedDomains[item]!.map((domain) => ({
            domainName: domain.url,
            isoCode: domain.culture,
          })),
        },
      });
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
