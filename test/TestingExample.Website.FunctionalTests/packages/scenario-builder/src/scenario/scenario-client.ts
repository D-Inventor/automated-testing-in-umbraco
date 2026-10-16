import {
  deleteDocumentById,
  getDocumentUrls,
  getTreeDocumentRoot,
  postDocument,
  postPublishedCacheReload,
  putDocumentByIdDomains,
  putDocumentByIdPublish,
} from '../client';
import { client } from '../client/client.gen';
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

type RequestOutcome<TData, TError> =
  | { data: TData; error: undefined; response?: Response }
  | { data: undefined; error: TError; response?: Response };

function throwOnFailure<TData, TError>(input: RequestOutcome<TData, TError>): TData {
  if (input.error || input.data === undefined) {
    throw new ApiError(input.response, input.error);
  }

  return input.data;
}

export async function createContentTree(sortedItems: ContentItem[]) {
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

export async function publishContentTree(sortedItems: ContentItem[]) {
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

export async function backfillContentUrls(sortedItems: ContentItem[]) {
  const result = throwOnFailure(
    await getDocumentUrls({
      query: {
        id: sortedItems.map((item) => item.id),
      },
    }),
  );

  result?.forEach((urlInfo) => {
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

export async function clearExistingContent() {
  const result = throwOnFailure(await getTreeDocumentRoot());

  for (const item of result?.items ?? []) {
    throwOnFailure(
      await deleteDocumentById({
        path: {
          id: item.id,
        },
      }),
    );
  }

  throwOnFailure(await postPublishedCacheReload());
}
