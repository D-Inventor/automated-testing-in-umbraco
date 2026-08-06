import { describe, it, expect } from 'vitest';
import { Scenario } from './scenario';

describe('Scenario', () => {
  it('builds the full URL from baseUrl and path', () => {
    const scenario = new Scenario({ baseUrl: 'http://localhost:5000', path: '/home' });
    expect(scenario.url).toBe('http://localhost:5000/home');
  });

  it('handles a root path', () => {
    const scenario = new Scenario({ baseUrl: 'http://localhost:5000', path: '/' });
    expect(scenario.url).toBe('http://localhost:5000/');
  });
});
