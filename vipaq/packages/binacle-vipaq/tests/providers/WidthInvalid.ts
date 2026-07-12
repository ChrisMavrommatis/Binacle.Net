// Ports C#: Providers/Width/WidthInvalidProvider.cs. Kind splits the rows into two sets, each a value its picker
// must reject: dimensions ("LxWxH") and coordinates ("X,Y,Z"). field is the offending field's PascalCase name. A
// row is one kind, never both. getCoordinatesWidth needs a full item, so coordinate cases carry a probe with
// dims defaulted to 1 (the picker reads only x/y/z).
// Not a *.test.ts file, so jest does not run it.

import {readVectors} from "../support/vectorReader";
import {parseDimensions, parseCoordinates} from "../support/vectorParser";
import {Coordinates, Dimensions} from "../../src/models";

type Item = Dimensions & Coordinates;

// Raw width-invalid.json row. Kind splits the rows and says how Values is parsed.
interface Vector {
	Name: string;
	Kind: "Dimensions" | "Coordinates";
	Values: string;
	Field: string;
}

// A resolved scenario: the parsed value the picker must reject, and the offending field's name.
export interface Scenario<TValue> {
	name: string;
	value: TValue;
	field: string;
}

function load(file: string): {dimensions: Scenario<Dimensions>[]; coordinates: Scenario<Item>[]} {
	const dimensions: Scenario<Dimensions>[] = [];
	const coordinates: Scenario<Item>[] = [];
	for (const vector of readVectors<Vector>(file)) {
		// Values is parsed by Kind, and the row lands in that kind's set only.
		if (vector.Kind === "Dimensions") {
			dimensions.push({name: vector.Name, value: parseDimensions(vector.Values), field: vector.Field});
		} else {
			coordinates.push({
				name: vector.Name,
				value: {length: 1, width: 1, height: 1, ...parseCoordinates(vector.Values)},
				field: vector.Field,
			});
		}
	}
	return {dimensions, coordinates};
}

export const {dimensions: dimensionCases, coordinates: coordinateCases} = load("width-invalid.json");
