import { ContentPage } from 'scenario-builder';

type DetailPageHeader = {
  title?: string;
  intro?: string;
};

export class DetailPage extends ContentPage {
  public hasHeader(variation: Variation, header: DetailPageHeader) {}
}
