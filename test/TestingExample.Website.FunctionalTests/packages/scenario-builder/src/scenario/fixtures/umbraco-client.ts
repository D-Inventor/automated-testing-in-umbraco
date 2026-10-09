import type { CreateDocumentRequestModel, UpdateDomainsRequestModel } from '@/client';
import type { Client } from '@/client/client';
import { createMswHandlers, type CreateMswHandlersResult } from '@/client/msw.gen';
import { setupServer, type SetupServer } from 'msw/node';

export class UmbracoClient {
  private handlers: CreateMswHandlersResult;
  private server: SetupServer;
  private BASE_URL: string;

  private _postDocumentRequests: CreateDocumentRequestModel[];
  private _putDomainsRequests: UpdateDomainsRequestModel[];
  private _postDocumentResponse?: Response;
  private _putDomainsResponse?: Response;

  constructor() {
    this.BASE_URL = 'https://localhost:3000';
    this.handlers = createMswHandlers({ baseUrl: this.BASE_URL });
    this.server = setupServer();

    this._postDocumentRequests = [];
    this._putDomainsRequests = [];
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

  public postDocumentResponse(value: Response) {
    this._postDocumentResponse = value;
  }

  public putDomainsResponse(value: Response) {
    this._putDomainsResponse = value;
  }

  public initialize(client: Client) {
    client.setConfig({ baseUrl: this.BASE_URL });
    this.server.listen({ onUnhandledFrame: 'error' });
  }

  public reset() {
    this.server.resetHandlers();
    this._postDocumentRequests = [];
    this._putDomainsRequests = [];
    this._postDocumentResponse = undefined;
    this._putDomainsResponse = undefined;
    this.server.use(
      this.handlers.pick.postDocument(({ request }) => {
        return request.json().then((body) => {
          this._postDocumentRequests.push(body);
          return this._postDocumentResponse ?? new Response(undefined, { status: 201 });
        });
      }),
      this.handlers.pick.putDocumentByIdDomains(({ request }) => {
        return request.json().then((body) => {
          this._putDomainsRequests.push(body);
          return this._putDomainsResponse ?? new Response(undefined, { status: 200 });
        });
      }),
    );
  }

  public close() {
    this.server.close();
  }
}
