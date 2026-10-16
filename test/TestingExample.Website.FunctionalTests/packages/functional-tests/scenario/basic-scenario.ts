import { ApiScenario, Invariant, type Scenario } from 'scenario-builder';
import { Homepage } from './domain/homepage';
import { Website } from './domain/website';
import { ContentRoot } from './domain/content-root';
import { SystemPages } from './domain/system-pages';
import { DetailPage } from './domain/detail-page';

type BasicScenario = {
  platform: Website;
  website: ContentRoot;
  homepage: Homepage;
  systemPages: SystemPages;
};

export const EnglishCulture: string = 'en-US';

export function basicScenario(): { content: BasicScenario; scenario: Scenario } {
  const scenario = new ApiScenario();

  const platform = new Website(scenario);
  platform.hasVariation(Invariant, 'Test website');
  platform.isPublishedIn(Invariant);

  const systemPages = new SystemPages(scenario);
  systemPages.hasOrder(2);
  systemPages.hasParent(platform);
  systemPages.hasVariation(Invariant, 'System pages');
  systemPages.isPublishedIn(Invariant);

  const notFoundPage = new DetailPage(scenario);
  notFoundPage.hasParent(systemPages);
  notFoundPage.hasVariation(Invariant, '404 Page not found');
  notFoundPage.hasHeader(Invariant, {
    intro: 'The content you are looking for does not exist.',
  });
  notFoundPage.isPublishedIn(Invariant);

  const serverErrorPage = new DetailPage(scenario);
  serverErrorPage.hasParent(systemPages);
  serverErrorPage.hasVariation(Invariant, '500 Internal server error');
  serverErrorPage.hasHeader(Invariant, {
    intro: 'Something went wrong while fetching this content.',
  });
  serverErrorPage.isPublishedIn(Invariant);

  const website = new ContentRoot(scenario);
  website.hasOrder(1);
  website.hasVariation(Invariant, 'website');
  website.hasParent(platform);
  website.hasDomain(EnglishCulture, new URL('https://localhost:44376'));
  website.isPublishedIn(Invariant);

  const homepage = new Homepage(scenario);
  homepage.hasParent(website);
  homepage.hasVariation(Invariant, 'Homepage');
  homepage.hasHeader(Invariant, { title: 'Welcome to our website' });
  homepage.isPublishedIn(Invariant);

  website.hasHomepage(Invariant, homepage);

  return {
    content: {
      platform: platform,
      website: website,
      homepage: homepage,
      systemPages: systemPages,
    },
    scenario: scenario,
  };
}
