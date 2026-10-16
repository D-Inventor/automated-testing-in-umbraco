import { vi } from 'vitest';
import type { ContentItem } from '../content-item';
import { backfillContentUrls, createContentTree, publishContentTree } from '../scenario-client';

type Operation = 'create' | 'publish' | 'backfill';

export class ScenarioClient {
  private _operationOrder: Operation[] = [];

  public get createdItems(): ContentItem[] | undefined {
    return vi.mocked(createContentTree).mock.lastCall?.[0];
  }

  public get publishedItems(): ContentItem[] | undefined {
    return vi.mocked(publishContentTree).mock.lastCall?.[0];
  }

  public get backfilledItems(): ContentItem[] | undefined {
    return vi.mocked(backfillContentUrls).mock.lastCall?.[0];
  }

  public get createdItemIds(): string[] | undefined {
    return this.createdItems?.map((item) => item.id);
  }

  public get operationOrder(): Operation[] {
    return this._operationOrder;
  }

  public createContentTreeFails(error: Error) {
    vi.mocked(createContentTree).mockRejectedValue(error);
  }

  public reset() {
    this._operationOrder = [];

    vi.mocked(createContentTree)
      .mockReset()
      .mockImplementation(async () => {
        this._operationOrder.push('create');
      });
    vi.mocked(publishContentTree)
      .mockReset()
      .mockImplementation(async () => {
        this._operationOrder.push('publish');
      });
    vi.mocked(backfillContentUrls)
      .mockReset()
      .mockImplementation(async () => {
        this._operationOrder.push('backfill');
      });
  }
}
