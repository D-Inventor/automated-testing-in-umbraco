import { randomUUID } from 'crypto';
import type { ContentItem } from '@/scenario/content-item';
import type { Variation } from './variation';

export interface Scenario {
  add(content: ContentItem): void;
}

export class ContentPage {
  private contentItem: ContentItem;
  private parentPage: ContentPage | undefined;
  private children: ContentPage[] = [];

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
      level: 0,
    };
    this.scenario.add(this.contentItem);
  }

  hasParent(parent: ContentPage): void {
    if (this.parentPage) {
      this.parentPage.children = this.parentPage.children.filter((c) => c !== this);
    }

    this.parentPage = parent;
    this.contentItem.parent = parent.id;

    parent.children.push(this);

    this.updateLevel();
  }

  private updateLevel(): void {
    const newLevel = (this.parentPage?.level ?? -1) + 1;

    if (this.contentItem.level !== newLevel) {
      this.contentItem.level = newLevel;
      this.notifyLevelHasChanged();
    }
  }

  private notifyLevelHasChanged(): void {
    for (const child of this.children) {
      child.updateLevel();
    }
  }

  get level(): number {
    return this.contentItem.level ?? 0;
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
