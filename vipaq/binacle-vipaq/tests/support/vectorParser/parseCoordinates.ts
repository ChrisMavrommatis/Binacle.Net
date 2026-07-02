import {Coordinates} from "../../../src/models";
import {parseThree} from "./parseThree";

// Ports C#: VectorParser.ParseCoordinates. "X,Y,Z" -> Coordinates (split on ',').
export function parseCoordinates(compact: string): Coordinates {
	const [x, y, z] = parseThree(compact, ",");
	return {x, y, z};
}
