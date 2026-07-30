// Ports C#: Providers/Width/WidthSelectionProvider.cs. Kind splits the rows into dimensions ("LxWxH") and
// coordinates ("X,Y,Z"); each runs through its own picker and must return expected. Both pickers use identical
// width math and the two sets together cover every bucket, so they can't drift. getCoordinatesWidth needs a full
// item, so coordinate cases carry a probe with dims defaulted to 1.
// Not a *.test.ts file, so jest does not run it.

import {readVectors} from "../support/vectorReader";
import {parseDimensions, parseCoordinates, parseWidth} from "../support/vectorParser";
import {Coordinates, Dimensions, Width} from "../../src/models";

type Item = Dimensions & Coordinates;

// Raw width-selection.json row. Kind splits the rows and says how Values is parsed.
interface Vector {
	Name: string;
	Kind: "Dimensions" | "Coordinates";
	Values: string;
	ExpectedWidth: string;
}

// A resolved scenario: the parsed value and the width the picker must choose for it.
export interface Scenario<TValue> {
	name: string;
	value: TValue;
	expected: Width;
}

function load(file: string): {dimensions: Scenario<Dimensions>[]; coordinates: Scenario<Item>[]} {
	const dimensions: Scenario<Dimensions>[] = [];
	const coordinates: Scenario<Item>[] = [];
	for (const vector of readVectors<Vector>(file)) {
		// Values is parsed by Kind, and the row lands in that kind's set only.
		if (vector.Kind === "Dimensions") {
			dimensions.push({name: vector.Name, value: parseDimensions(vector.Values), expected: parseWidth(vector.ExpectedWidth)});
		} else {
			coordinates.push({
				name: vector.Name,
				value: {length: 1, width: 1, height: 1, ...parseCoordinates(vector.Values)},
				expected: parseWidth(vector.ExpectedWidth),
			});
		}
	}
	return {dimensions, coordinates};
}

export const {dimensions: dimensionCases, coordinates: coordinateCases} = load("width/width-selection.json");
