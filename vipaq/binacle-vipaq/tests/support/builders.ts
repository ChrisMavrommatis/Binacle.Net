// Shared test data builders. Curated literals, no faker — inputs are deterministic so golden byte
// tests stay exact. Use anItem({...}) to override only the field a test cares about.
//
// Not a *.test.ts file, so jest does not run it.

import {Coordinates, Dimensions} from "../../src/models";

export type Item = Dimensions & Coordinates;

export function bin(length: number, width: number, height: number): Dimensions {
	return {length, width, height};
}

export function item(l: number, w: number, h: number, x: number, y: number, z: number): Item {
	return {length: l, width: w, height: h, x, y, z};
}

export function anItem(overrides: Partial<Item> = {}): Item {
	return {length: 1, width: 1, height: 1, x: 0, y: 0, z: 0, ...overrides};
}
