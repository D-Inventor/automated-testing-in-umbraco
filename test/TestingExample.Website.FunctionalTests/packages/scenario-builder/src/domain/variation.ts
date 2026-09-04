export type Variation = {
  culture: Culture;
  segment: Segment;
};

export const InvariantCulture = null;

// TODO: what is the actual format for culture?
export type Culture = string | typeof InvariantCulture;

export const InvariantSegment = null;

export type Segment = string | typeof InvariantSegment;

export const Invariant: Variation = {
  culture: InvariantCulture,
  segment: InvariantSegment,
};

export function cultureVariant(culture: string): Variation {
  return {
    culture: culture,
    segment: InvariantSegment,
  };
}
