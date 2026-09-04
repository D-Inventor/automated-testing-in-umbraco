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
    };
    this.scenario.add(this.contentItem);
  }

  hasParent(parent: ContentPage): void {
    this.contentItem.parent = parent.id;
  }
  hasDomain(culture: Culture, url: URL): void {}
  hasVariation(variation: Variation, name: string): void {}
  isPublishedIn(variation: Variation): void {}

  get id(): string {
    return this.contentItem.id;
  }
}
