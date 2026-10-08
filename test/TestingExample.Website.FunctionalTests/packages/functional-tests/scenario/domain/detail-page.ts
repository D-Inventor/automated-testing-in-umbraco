import { ContentPage, type Scenario, type Variation } from 'scenario-builder';

type DetailPageHeader = {
  title?: string;
  intro?: string;
};

export class DetailPage extends ContentPage {
  constructor(scenario: Scenario) {
    super(scenario, 'cd9e9f2c-64f3-4723-9bd8-d5d362544dd5');
  }

  public hasHeader(_variation: Variation, _header: DetailPageHeader) {}
}
