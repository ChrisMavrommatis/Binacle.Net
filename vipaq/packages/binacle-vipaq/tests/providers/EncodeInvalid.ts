// Ports C#: Providers/EncodeInvalidProvider.cs. A (bin, items) input the serializer must reject end-to-end.
// The `:Q` suffix expands the item-count scenario to 65536 items without listing them.
// Not a *.test.ts file, so jest does not run it.

import {readVectors} from "../support/vectorReader";
import {parseBin, parseItems} from "../support/vectorParser";
import {Coordinates, Dimensions} from "../../src/models";

type Item = Dimensions & Coordinates;

// Raw encode-invalid.json row. Reason is documentation only.
interface Vector {
	Name: string;
	Bin: string;
	Items: string[];
	Reason: string;
}

// A resolved scenario: the (bin, items) input the serializer must reject.
export interface Scenario {
	name: string;
	bin: Dimensions;
	items: Item[];
}

function load(file: string): Scenario[] {
	const scenarios: Scenario[] = [];
	for (const vector of readVectors<Vector>(file)) {
		scenarios.push({name: vector.Name, bin: parseBin(vector.Bin), items: parseItems(vector.Items)});
	}
	return scenarios;
}

export const encodeInvalidCases: Scenario[] = load("serialization/encode-invalid.json");
