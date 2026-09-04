import type { Variation } from '@/domain/variation';

export type ContentItem = {
  id: string;
  parent?: string;
  documentType: string;
  template?: string;
  values: ContentItemValue[];
  variants: ContentItemVariant[];
};

export type ContentItemVariant = {
  variation: Variation;
  name: string;
};

export type ContentItemValue = {
  variation: Variation;
  alias: string;
  value?: unknown;
};
