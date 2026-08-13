import { ContentPage } from 'scenario-builder';

type ContentRootErrorPages = {
  notFound: ContentPage;
  serverError: ContentPage;
};

export class ContentRoot extends ContentPage {
  public hasErrorPages(variation: Variation, errorPages: ContentRootErrorPages) {}
}
