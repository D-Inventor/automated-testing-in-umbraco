import { ContentPage, type Scenario, type Variation } from 'scenario-builder';
import type { Homepage } from './homepage';

export class ContentRoot extends ContentPage {
  constructor(scenario: Scenario) {
    super(scenario, '97001531-6692-4e9c-a64a-3295c86b981d');
  }

  public hasHomepage(variation: Variation, page: Homepage) {
    this.hasValue(variation, 'umbracoInternalRedirectId', page.id);
  }
}
