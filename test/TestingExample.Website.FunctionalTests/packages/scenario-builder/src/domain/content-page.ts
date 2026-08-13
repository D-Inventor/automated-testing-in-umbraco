import type { Scenario } from '@/scenario';

export class ContentPage {
  constructor(private scenario: Scenario) {}

  hasParent(parent: ContentPage): void {}
  hasDomain(culture: Culture, url: URL): void {}
  hasVariation(variation: Variation, name: string): void {}
  isPublishedIn(variation: Variation): void {}
}
