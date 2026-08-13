import { Scenario } from 'scenario-builder';
import { Homepage } from './domain/homepage';
import { Website } from './domain/website';
import { ContentRoot } from './domain/content-root';
import { SystemPages } from './domain/system-pages';
import { DetailPage } from './domain/detail-page';

export function basicScenario(): Scenario {
  let scenario = new Scenario({ baseUrl: 'https://playwright.dev', path: '/' });

  const platform = new Website(scenario);
  platform.hasVariation(English, 'Test website');
  platform.isPublishedIn(English);

  const systemPages = new SystemPages(scenario);
  systemPages.hasParent(platform);
  systemPages.hasVariation(English, 'System pages');
  systemPages.isPublishedIn(English);

  const notFoundPage = new DetailPage(scenario);
  notFoundPage.hasParent(systemPages);
  notFoundPage.hasVariation(English, '404 Page not found');
  notFoundPage.hasHeader(English, {
    title: '404 Page not found',
    intro: 'The content you are looking for does not exist.',
  });
  notFoundPage.isPublishedIn(English);

  const serverErrorPage = new DetailPage(scenario);
  serverErrorPage.hasParent(systemPages);
  serverErrorPage.hasVariation(English, '500 Internal server error');
  serverErrorPage.hasHeader(English, {
    title: '500 Internal server error',
    intro: 'Something went wrong while fetching this content.',
  });
  serverErrorPage.isPublishedIn(English);

  const website = new ContentRoot(scenario);
  website.hasVariation(English, 'website');
  website.hasParent(platform);
  website.hasDomain(EnglishCulture, 'https://localhost:44356');
  website.hasErrorPages(English, {
    notFound: notFoundPage,
    serverError: serverErrorPage,
  });
  website.isPublishedIn(English);

  const homepage = new Homepage(scenario);
  homepage.hasParent(website);
  homepage.hasVariation(English, 'Homepage');
  homepage.hasHeader(English, { title: 'Welcome to our website' });
  homepage.isPublishedIn(English);

  return scenario;
}
