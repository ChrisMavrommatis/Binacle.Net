// Ports C#: Providers/DecodeInvalidProvider.cs. A raw blob the decoder must reject. Consumed by
// decodeInvalid.test.ts. Not a *.test.ts file, so jest does not run it.

import {readVectors} from "../support/vectorReader";
import {parseBytes} from "../support/vectorParser";

// Raw decode-invalid.json row. Reason is documentation only — each language rejects for its own reason.
interface Vector {
	Name: string;
	Blob: string[];
	Reason: string;
}

// A resolved scenario: the raw blob the decoder must reject.
export interface Scenario {
	name: string;
	blob: number[];
}

function load(file: string): Scenario[] {
	const scenarios: Scenario[] = [];
	for (const vector of readVectors<Vector>(file)) {
		scenarios.push({name: vector.Name, blob: parseBytes(vector.Blob)});
	}
	return scenarios;
}

export const decodeInvalidCases: Scenario[] = load("serialization/decode-invalid.json");
