import {Dimensions} from "../../../src/models";
import {parseThree} from "./parseThree";

// Ports C#: VectorParser.ParseDimensions. "LxWxH" -> Dimensions (split on 'x').
export function parseDimensions(compact: string): Dimensions {
	const [length, width, height] = parseThree(compact, "x");
	return {length, width, height};
}
