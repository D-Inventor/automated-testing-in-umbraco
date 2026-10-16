import type { RequestResult } from '../client/client';
import {
  getDocumentUrls,
  postDocument,
  putDocumentByIdDomains,
  putDocumentByIdPublish,
} from '../client';
import type { ContentItem } from './content-item';
import { convertToContentPostRequest } from './mappers';
import { client } from '@/client/client.gen';

class ApiError extends Error {
  constructor(
    public response: Response | undefined,
    error: unknown,
  ) {
    super(
      `Response failed [${response?.status}: ${response?.statusText}]\n${JSON.stringify(error, null, 2)}`,
    );
    this.name = 'ApiError';
  }
}

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

    await this.createContentTree(sortedItems);

    await this.publishContentTree(sortedItems);

    await this.fetchContentUrls(sortedItems);
  }

  private async fetchContentUrls(sortedItems: ContentItem[]) {
    const result = throwOnFailure(
      await getDocumentUrls({
        query: {
          id: sortedItems.map((item) => item.id),
        },
      }),
    );

    result.data?.forEach((urlInfo) => {
      const contentItem = sortedItems.find((item) => item.id === urlInfo.id);
      if (contentItem)
        contentItem.urls = urlInfo.urlInfos
          .filter((info) => !!info.url && !!info.culture)
          .map((info) => ({
            culture: info.culture!,
            url: new URL(info.url!, client.getConfig().baseUrl),
          }));
    });
  }

  private async publishContentTree(sortedItems: ContentItem[]) {
    for (const item of sortedItems) {
      if (item.published && item.published.length > 0) {
        throwOnFailure(
          await putDocumentByIdPublish({
            path: {
              id: item.id,
            },
            body: {
              publishSchedules: item.published.map((variation) => ({
                culture: variation.culture,
              })),
            },
          }),
        );
      }
    }
  }

  private async createContentTree(sortedItems: ContentItem[]) {
    for (const item of sortedItems) {
      throwOnFailure(
        await postDocument({
          body: convertToContentPostRequest(item),
        }),
      );

      if (item.domains && item.domains.length > 0) {
        throwOnFailure(
          await putDocumentByIdDomains({
            path: {
              id: item.id,
            },
            body: {
              domains: item.domains.map((domain) => ({
                domainName: domain.url,
                isoCode: domain.culture,
              })),
            },
          }),
        );
      }
    }
  }
}

function throwOnFailure<T, U>(
  input: Awaited<RequestResult<T, U, false>>,
): Awaited<RequestResult<T, U, false>> {
  if (input.error) {
    throw new ApiError(input.response, input.error);
  }

  return input;
}
