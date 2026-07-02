import {Coordinates, Dimensions} from "../../../src/models";
import {parseDimensions} from "./parseDimensions";
import {parseCoordinates} from "./parseCoordinates";

type Item = Dimensions & Coordinates;

// Ports C#: VectorParser.ParseItems. "LxWxH (X,Y,Z):Q" -> Q copies of the item (Q optional, default 1).
// ':' is the quantity separator (not '-') so '-' stays free for negative dims/coords.
export function parseItems(compactItems: string[]): Item[] {
	const result: Item[] = [];
	for (const compact of compactItems) {
		result.push(...parseItem(compact));
	}
	return result;
}

function parseItem(compact: string): Item[] {
	let quantity = 1;
	let body = compact;

	const colon = compact.indexOf(":");
	if (colon >= 0) {
		body = compact.slice(0, colon);
		quantity = Number(compact.slice(colon + 1));
	}

	const space = body.indexOf(" ");
	if (space < 0) throw new Error(`Item '${compact}' must be 'LxWxH (X,Y,Z)'.`);

	const dimensions = parseDimensions(body.slice(0, space));
	const coordinatesText = body.slice(space + 1).trim().replace(/^\(/, "").replace(/\)$/, "");
	const coordinates = parseCoordinates(coordinatesText);

	const items: Item[] = [];
	for (let i = 0; i < quantity; i++) {
		items.push({...dimensions, ...coordinates});
	}
	return items;
}
