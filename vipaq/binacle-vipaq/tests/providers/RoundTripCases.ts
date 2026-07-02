// Ports C#: Providers/RoundTripProvider.cs. A (bin, items) input plus the header the serializer must
// produce. Read row by row, resolved into Scenario objects. Consumed by roundTrip.test.ts.
// Not a *.test.ts file, so jest does not run it.

import {readVectors} from "../support/vectorReader";
import {parseBin, parseItems, parseEncodingInfo} from "../support/vectorParser";
import {Coordinates, Dimensions, EncodingInfo} from "../../src/models";

type Item = Dimensions & Coordinates;

// Raw round-trip-scenarios.json row.
interface Vector {
	Name: string;
	Bin: string;
	Items: string[];
	ExpectedEncodingInfo: string;
}

// A resolved scenario: the parsed (bin, items) and the header byte 0 the serializer must produce.
export interface Scenario {
	name: string;
	bin: Dimensions;
	items: Item[];
	expected: EncodingInfo;
}

function load(file: string): Scenario[] {
	const scenarios: Scenario[] = [];
	for (const vector of readVectors<Vector>(file)) {
		scenarios.push({
			name: vector.Name,
			bin: parseBin(vector.Bin),
			items: parseItems(vector.Items),
			expected: parseEncodingInfo(vector.ExpectedEncodingInfo),
		});
	}
	return scenarios;
}

export const roundTripCases: Scenario[] = load("round-trip-scenarios.json");
