// Ports C#: Providers/Header/HeaderBytesProvider.cs. Every Header combo (all 32) and the two header bytes it
// packs to. `bytes` is an independent golden — the parser building `header` never sees it, so the test is a real
// check, not a round-trip. Consumed by tests/utils/header.test.ts.
// Not a *.test.ts file, so jest does not run it.

import {readVectors} from "../support/vectorReader";
import {parseHeader, parseBytes} from "../support/vectorParser";
import {Header} from "../../src/models";

// Raw header-bytes.json row: the header notation and the two header bytes it packs to.
interface Vector {
	Header: string;
	Bytes: string[];
}

// A resolved scenario: the parsed Header, its two golden bytes, and the notation as the test label.
export interface Scenario {
	notation: string;
	header: Header;
	bytes: number[];
}

function load(file: string): Scenario[] {
	const scenarios: Scenario[] = [];
	for (const vector of readVectors<Vector>(file)) {
		scenarios.push({
			notation: vector.Header,
			header: parseHeader(vector.Header),
			bytes: parseBytes(vector.Bytes),
		});
	}
	return scenarios;
}

export const headerBytesCases: Scenario[] = load("header/header-bytes.json");
