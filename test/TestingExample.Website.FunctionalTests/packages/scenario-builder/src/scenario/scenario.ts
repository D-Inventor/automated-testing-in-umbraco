import type { ContentItem } from './content-item';
import { backfillContentUrls, createContentTree, publishContentTree } from './scenario-client';

export interface Scenario {
  add(content: ContentItem): void;

  build(): Promise<void>;
}

export class ApiScenario implements Scenario {
  private added: Record<string, ContentItem> = {};

  public add(content: ContentItem) {
    this.added[content.id] = content;
  }

  public async build(): Promise<void> {
    const sortedItems = Object.values(this.added).sort(
      (a, b) => (a.level ?? 0) - (b.level ?? 0) || (a.order ?? 0) - (b.order ?? 0),
    );

    await createContentTree(sortedItems);

    await publishContentTree(sortedItems);

    await backfillContentUrls(sortedItems);
  }
}
