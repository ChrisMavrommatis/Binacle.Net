// Ports C#: Providers/Protocol/LittleEndianProvider.cs. A value paired with its little-endian bytes (low byte
// first). Reader and writer tests run the same rows. One export per width; only the 8- and 16-bit widths exist
// now (the 32/64-bit vectors are gone with those tiers).
// Not a *.test.ts file, so jest does not run it.

import {readVectors} from "../support/vectorReader";
import {parseBytes} from "../support/vectorParser";

// Raw little-endian/<width>.json row.
interface Vector {
	Name: string;
	Value: string;
	Bytes: string[];
}

// A resolved scenario: a value and the little-endian bytes it occupies on the wire.
export interface Scenario {
	name: string;
	value: number;
	bytes: number[];
}

function load(file: string): Scenario[] {
	const scenarios: Scenario[] = [];
	for (const vector of readVectors<Vector>(file)) {
		scenarios.push({
			name: vector.Name,
			value: Number(vector.Value), // "0x..." parses as a JS number; every shared row is within 2^53
			bytes: parseBytes(vector.Bytes),
		});
	}
	return scenarios;
}

export const uint8Cases: Scenario[] = load("protocol/little-endian/uint8.json");
export const uint16Cases: Scenario[] = load("protocol/little-endian/uint16.json");
