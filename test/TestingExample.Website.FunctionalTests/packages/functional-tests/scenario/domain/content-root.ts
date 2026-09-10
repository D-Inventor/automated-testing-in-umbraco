import { ContentPage, type Scenario, type Variation } from 'scenario-builder';

type ContentRootErrorPages = {
  notFound: ContentPage;
  serverError: ContentPage;
};

export class ContentRoot extends ContentPage {
  constructor(scenario: Scenario) {
    super(scenario, '97001531-6692-4e9c-a64a-3295c86b981d');
  }

  public hasErrorPages(variation: Variation, errorPages: ContentRootErrorPages) {}
}
