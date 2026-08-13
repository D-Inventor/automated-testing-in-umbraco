import { ContentPage } from 'scenario-builder';

type HomepageHeader = {
  title?: string;
  description?: string;
};

export class Homepage extends ContentPage {
  public hasHeader(variation: Variation, header: HomepageHeader): Homepage {
    return this;
  }
}
