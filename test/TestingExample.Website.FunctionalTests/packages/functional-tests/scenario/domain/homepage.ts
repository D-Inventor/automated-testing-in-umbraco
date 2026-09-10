import { ContentPage, type Scenario, type Variation } from 'scenario-builder';

type HomepageHeader = {
  title?: string;
  description?: string;
};

export class Homepage extends ContentPage {
  constructor(scenario: Scenario) {
    super(scenario, 'f0cf962b-6398-477c-aa04-e4fbb4d69162');
  }
  public hasHeader(variation: Variation, header: HomepageHeader): Homepage {
    return this;
  }
}
