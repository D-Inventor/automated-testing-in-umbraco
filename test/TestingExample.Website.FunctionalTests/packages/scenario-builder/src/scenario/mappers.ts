import type { ContentItem, ContentItemValue, ContentItemVariant } from './content-item';
import type {
  CreateDocumentRequestModel,
  DocumentValueModel,
  DocumentVariantRequestModel,
  ReferenceByIdModel,
} from '../client';

export function convertToContentPostRequest(item: ContentItem): CreateDocumentRequestModel {
  const result = {
    id: item.id,
    parent: item.parent !== undefined ? convertToReference(item.parent) : null,
    documentType: convertToReference(item.documentType),
    template: item.template !== undefined ? convertToReference(item.template) : null,
    values: item.values.map(convertToRequestValue),
    variants: item.variants.map(convertToRequestVariant),
  };

  return result;
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
