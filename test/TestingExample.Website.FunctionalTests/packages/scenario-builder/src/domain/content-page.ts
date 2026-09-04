import { randomUUID } from 'crypto';
import type { ContentItem } from '@/scenario/content-item';
import type { Culture, Variation } from './variation';

export interface Scenario {
  add(content: ContentItem): void;
}

export class ContentPage {
  private contentItem: ContentItem;

  constructor(
    private scenario: Scenario,
    private contenttype: string,
  ) {
    this.contentItem = {
      id: randomUUID(),
      documentType: this.contenttype,
      values: [],
      variants: [],
      domains: [],
      published: [],
    };
    this.scenario.add(this.contentItem);
  }

  hasParent(parent: ContentPage): void {
    this.contentItem.parent = parent.id;
  }

  hasDomain(culture: string, url: URL): void {
    this.contentItem.domains!.push({ culture, url: url.toString() });
  }

  hasVariation(variation: Variation, name: string): void {
    this.contentItem.variants.push({ variation, name });
  }

  isPublishedIn(variation: Variation): void {
    const hasVariant = this.contentItem.variants.some(
      (v) => v.variation.culture === variation.culture && v.variation.segment === variation.segment,
    );
    if (!hasVariant) {
      throw new Error(
        `Variation not added: culture="${variation.culture}", segment=${variation.segment}`,
      );
    }
    this.contentItem.published!.push(variation);
  }

  get id(): string {
    return this.contentItem.id;
  }
}
