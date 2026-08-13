// Ports C#: Providers/Serialization/RoundTripProvider.cs. A (bin, items) input plus the header the encoder must
// produce, in header-notation text form.
// Not a *.test.ts file, so jest does not run it.

import {readVectors} from "../support/vectorReader";
import {parseBin, parseItems, parseHeader} from "../support/vectorParser";
import {Coordinates, Dimensions, Header} from "../../src/models";

type Item = Dimensions & Coordinates;

// Raw round-trip-scenarios.json row.
interface Vector {
	Name: string;
	Bin: string;
	Items: string[];
	ExpectedHeader: string;
}

// A resolved scenario: the parsed (bin, items) and the header the encoder must produce.
export interface Scenario {
	name: string;
	bin: Dimensions;
	items: Item[];
	expected: Header;
}

function load(file: string): Scenario[] {
	const scenarios: Scenario[] = [];
	for (const vector of readVectors<Vector>(file)) {
		scenarios.push({
			name: vector.Name,
			bin: parseBin(vector.Bin),
			items: parseItems(vector.Items),
			expected: parseHeader(vector.ExpectedHeader),
		});
	}
	return scenarios;
}

export const roundTripCases: Scenario[] = load("serialization/round-trip-scenarios.json");
