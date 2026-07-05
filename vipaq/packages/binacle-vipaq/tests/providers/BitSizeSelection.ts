// Ports C#: Providers/BitSizeSelectionProvider.cs. Kind splits the rows into dimensions ("LxWxH") and
// coordinates ("X,Y,Z"); each runs through its own picker and must return expected. Both pickers use
// identical width math and the two sets together cover every bucket, so they can't drift.
// getCoordinatesBitSize needs a full item, so coordinate cases carry a probe with dims defaulted to 1.
// Not a *.test.ts file, so jest does not run it.

import {readVectors} from "../support/vectorReader";
import {parseDimensions, parseCoordinates, parseBitSize} from "../support/vectorParser";
import {BitSize, Coordinates, Dimensions} from "../../src/models";

type Item = Dimensions & Coordinates;

// Raw bit-size-selection.json row. Kind splits the rows and says how Values is parsed.
interface Vector {
	Name: string;
	Kind: "Dimensions" | "Coordinates";
	Values: string;
	ExpectedBitSize: string;
}

// A resolved scenario (mirrors the C# generic Scenario<TValue> record): the parsed value and the width the
// picker must choose for it.
export interface Scenario<TValue> {
	name: string;
	value: TValue;
	expected: BitSize;
}

function load(file: string): {dimensions: Scenario<Dimensions>[]; coordinates: Scenario<Item>[]} {
	const dimensions: Scenario<Dimensions>[] = [];
	const coordinates: Scenario<Item>[] = [];
	for (const vector of readVectors<Vector>(file)) {
		// Values is parsed by Kind, and the row lands in that kind's set only.
		if (vector.Kind === "Dimensions") {
			dimensions.push({name: vector.Name, value: parseDimensions(vector.Values), expected: parseBitSize(vector.ExpectedBitSize)});
		} else {
			coordinates.push({
				name: vector.Name,
				value: {length: 1, width: 1, height: 1, ...parseCoordinates(vector.Values)},
				expected: parseBitSize(vector.ExpectedBitSize),
			});
		}
	}
	return {dimensions, coordinates};
}

export const {dimensions: dimensionCases, coordinates: coordinateCases} = load("bit-size-selection.json");
