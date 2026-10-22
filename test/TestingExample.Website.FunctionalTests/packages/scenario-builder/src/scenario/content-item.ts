import type { LanguageCulture, Variation } from '../domain/variation';

export type ContentItemDomain = {
  culture: LanguageCulture;
  url: string;
};

export type ContentItem = {
  id: string;
  parent?: string;
  documentType: string;
  template?: string;
  values: ContentItemValue[];
  variants: ContentItemVariant[];
  domains: ContentItemDomain[];
  published: Variation[];
  urls: { culture: LanguageCulture; url: URL }[];
  level: number;
  order: number;
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
