import type { RequestResult } from '../client/client';
import {
  postDocument,
  putDocumentByIdDomains,
  putDocumentByIdPublish,
} from '../client';
import type { ContentItem } from './content-item';
import { convertToContentPostRequest } from './mappers';

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

    // Post all documents first
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

    // Publish documents after all have been posted
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
}

function throwOnFailure<T, U>(
  input: Awaited<RequestResult<T, U, false>>,
): Awaited<RequestResult<T, U, false>> {
  if (input.error) {
    throw new ApiError(input.response, input.error);
  }

  return input;
}
