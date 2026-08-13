export interface ScenarioOptions {
  baseUrl: string;
  path: string;
}

export class Scenario {
  readonly url: string;

  constructor(options: ScenarioOptions) {
    this.url = `${options.baseUrl}${options.path}`;
  }

  public get<T>(key: string): T {}
}
