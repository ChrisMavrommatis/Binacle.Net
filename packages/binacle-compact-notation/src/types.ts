// The three geometry blocks as plain shapes. TS is structural, so any object carrying these fields
// satisfies the type — a consumer's own model (e.g. vipaq's) is assignable with no mapping.

export interface Dimensions {
	length: number;
	width: number;
	height: number;
}

export interface Coordinates {
	x: number;
	y: number;
	z: number;
}

export type Item = Dimensions & Coordinates;

// Which block a compact string is, as decided by detect().
export type CompactNotationKind = "dimensions" | "coordinates" | "quantity";
