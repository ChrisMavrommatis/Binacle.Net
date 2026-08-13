// Ports C#: Providers/ExactBytesProvider.cs. A named (bin, items) input paired with the exact wire bytes
// the serializer must produce. The Bytes object mirrors the wire layout by segment; flatten() joins it into
// one blob: Header :: Count :: Bin :: (Dims :: Coords per item). Consumed by ViPaqSerializer.test.ts.
// Not a *.test.ts file, so jest does not run it.

import {readVectors} from "../support/vectorReader";
import {parseBin, parseItems, parseBytes} from "../support/vectorParser";
import {Coordinates, Dimensions} from "../../src/models";

type Item = Dimensions & Coordinates;

// Raw exact-bytes.json row: the (bin, items) input and the exact wire bytes laid out by segment. Header is the
// two form/width bytes (PROTOCOL.md §2).
interface Vector {
	Name: string;
	Bin: string;
	Items: string[];
	Bytes: {
		Header: string[];
		Count: string[];
		Bin: string[];
		Items: {Dims: string[]; Coords: string[]}[];
	};
}

// A resolved scenario: the parsed (bin, items) and the flattened golden wire bytes.
export interface Scenario {
	name: string;
	bin: Dimensions;
	items: Item[];
	bytes: number[];
}

// Flattens the by-segment golden into one wire blob: Header (2 bytes) :: Count :: Bin :: (Dims :: Coords per item).
function flatten(bytes: Vector["Bytes"]): number[] {
	const result = [...parseBytes(bytes.Header), ...parseBytes(bytes.Count), ...parseBytes(bytes.Bin)];
	for (const item of bytes.Items) {
		result.push(...parseBytes(item.Dims), ...parseBytes(item.Coords));
	}
	return result;
}

function load(file: string): Scenario[] {
	const scenarios: Scenario[] = [];
	for (const vector of readVectors<Vector>(file)) {
		scenarios.push({
			name: vector.Name,
			bin: parseBin(vector.Bin),
			items: parseItems(vector.Items),
			bytes: flatten(vector.Bytes),
		});
	}
	return scenarios;
}

export const exactBytesCases: Scenario[] = load("serialization/exact-bytes.json");
