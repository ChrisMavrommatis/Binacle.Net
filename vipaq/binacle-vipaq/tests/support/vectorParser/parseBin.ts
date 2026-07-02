import {Dimensions} from "../../../src/models";
import {parseDimensions} from "./parseDimensions";

// Ports C#: VectorParser.ParseBin. A bin is just a Dimensions here (no separate Bin type in TS).
export function parseBin(compact: string): Dimensions {
	return parseDimensions(compact);
}
