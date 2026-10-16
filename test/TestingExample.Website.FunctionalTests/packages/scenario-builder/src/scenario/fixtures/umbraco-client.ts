import { setupServer, type SetupServer } from 'msw/node';
import { HttpResponse } from 'msw';
import { createMswHandlers, type CreateMswHandlersResult } from '../../client/msw.gen';
import type {
  CreateDocumentRequestModel,
  DocumentTreeItemResponseModel,
  DocumentUrlInfoModel,
  UpdateDomainsRequestModel,
} from '../../client';
import type { LanguageCulture } from '../../domain/variation';
import type { Client } from '../../client/client';

export class UmbracoClient {
  private handlers: CreateMswHandlersResult;
  private server: SetupServer;
  private BASE_URL: string;

  private _postDocumentRequests: CreateDocumentRequestModel[];
  private _putDomainsRequests: UpdateDomainsRequestModel[];
  private _publishDocumentIds: string[];
  private _deletedContentIds: string[];
  private _operationOrder: Array<'post' | 'publish' | 'fetchUrls'>;
  private _postDocumentResponse?: Response;
  private _putDomainsResponse?: Response;
  private _putPublishResponse?: Response;
  private _contentUrlsResponses: Map<string, DocumentUrlInfoModel[]>;
  private _contentAtRoot: DocumentTreeItemResponseModel[];

  constructor() {
    this.BASE_URL = 'https://localhost:3000';
    this.handlers = createMswHandlers({ baseUrl: this.BASE_URL });
    this.server = setupServer();

    this._postDocumentRequests = [];
    this._putDomainsRequests = [];
    this._publishDocumentIds = [];
    this._deletedContentIds = [];
    this._operationOrder = [];
    this._contentUrlsResponses = new Map();
    this._contentAtRoot = [];
  }

  public get lastPostDocument(): CreateDocumentRequestModel | undefined {
    return this._postDocumentRequests.at(-1);
  }

  public get lastPutDomains(): UpdateDomainsRequestModel | undefined {
    return this._putDomainsRequests.at(-1);
  }

  public get postedDocumentIds(): (string | null | undefined)[] {
    return this._postDocumentRequests.map((req) => req.id);
  }

  public get publishedDocumentIds(): string[] {
    return this._publishDocumentIds;
  }

  public get deletedContentIds(): string[] {
    return this._deletedContentIds;
  }

  public get operationOrder(): Array<'post' | 'publish' | 'fetchUrls'> {
    return this._operationOrder;
  }

  public postDocumentResponse(value: Response) {
    this._postDocumentResponse = value;
  }

  public putDomainsResponse(value: Response) {
    this._putDomainsResponse = value;
  }

  public putPublishResponse(value: Response) {
    this._putPublishResponse = value;
  }

  public contentUrlsResponse(contentId: string, culture: LanguageCulture, urls: string[]) {
    let infos = this._contentUrlsResponses.get(contentId) ?? [];

    infos = infos.filter((info) => info.culture !== culture);
    urls.forEach((url) =>
      infos.push({ culture: culture, message: null, provider: 'umbDocumentUrlProvider', url: url }),
    );

    this._contentUrlsResponses.set(contentId, infos);
  }

  public contentAtRootIs(items: DocumentTreeItemResponseModel[]) {
    this._contentAtRoot = items;
  }

  public absoluteUrl(relativeUrl: string): URL {
    return new URL(relativeUrl, this.BASE_URL);
  }

  public initialize(client: Client) {
    client.setConfig({ baseUrl: this.BASE_URL });
    this.server.listen({ onUnhandledFrame: 'error' });
  }

  public reset() {
    this.server.resetHandlers();
    this._postDocumentRequests = [];
    this._putDomainsRequests = [];
    this._publishDocumentIds = [];
    this._deletedContentIds = [];
    this._operationOrder = [];
    this._postDocumentResponse = undefined;
    this._putDomainsResponse = undefined;
    this._putPublishResponse = undefined;
    this._contentUrlsResponses.clear();
    this._contentAtRoot = [];
    this.server.use(
      this.handlers.pick.postDocument(({ request }) => {
        return request.json().then((body) => {
          this._postDocumentRequests.push(body);
          this._operationOrder.push('post');
          return this._postDocumentResponse ?? new Response(undefined, { status: 201 });
        });
      }),
      this.handlers.pick.putDocumentByIdDomains(({ request }) => {
        return request.json().then((body) => {
          this._putDomainsRequests.push(body);
          return this._putDomainsResponse ?? new Response(undefined, { status: 200 });
        });
      }),
      this.handlers.pick.putDocumentByIdPublish(({ params }) => {
        this._publishDocumentIds.push(params.id as string);
        this._operationOrder.push('publish');
        return this._putPublishResponse ?? new Response(undefined, { status: 200 });
      }),
      this.handlers.pick.getDocumentUrls(({ request }) => {
        const searchParams = new URL(request.url).searchParams;
        const ids = searchParams.getAll('id');

        const responseModel = ids.map((id) => ({
          id: id,
          urlInfos: this._contentUrlsResponses.get(id) ?? [],
        }));

        return HttpResponse.json(responseModel, { status: 200 });
      }),
      this.handlers.pick.getTreeDocumentRoot(() => {
        return HttpResponse.json(this._contentAtRoot, { status: 200 });
      }),
      this.handlers.pick.deleteDocumentById(({ params }) => {
        this._deletedContentIds.push(params.id as string);
        return new Response(undefined, { status: 200 });
      }),
    );
  }

  public close() {
    this.server.close();
  }
}
