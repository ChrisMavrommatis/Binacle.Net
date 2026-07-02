import {Coordinates, Dimensions} from "../src/models";

// Turns the shared vectors' compact strings into the plain objects the TS serializer takes, following
// the same rules as the C# generator's CompactParser (see vipaq/test-vectors/README.md):
//   bin / dimensions — "LxWxH"  (split on 'x')
//   coordinates      — "X,Y,Z"  (split on ',')
//   item             — "LxWxH (X,Y,Z):Q"  where ":Q" repeats the item Q times (default 1)
// Each part owns its separator so a coordinate is never read as a dimension. Kept minimal on purpose so
// the tool has no dependency on the test support parser — it stands alone like the C# tool does.

type Item = Dimensions & Coordinates;

export function parseBin(compact: string): Dimensions {
	const [length, width, height] = parseThree(compact, "x");
	return {length, width, height};
}

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

	// Split off the ":Q" repeat suffix first, so a leading '-' stays free for negative dims/coords.
	const colon = compact.indexOf(":");
	if (colon >= 0) {
		body = compact.slice(0, colon);
		quantity = Number(compact.slice(colon + 1));
	}

	const space = body.indexOf(" ");
	if (space < 0) throw new Error(`Item '${compact}' must be 'LxWxH (X,Y,Z)'.`);

	const [length, width, height] = parseThree(body.slice(0, space), "x");
	const coordinatesText = body.slice(space + 1).trim().replace(/^\(/, "").replace(/\)$/, "");
	const [x, y, z] = parseThree(coordinatesText, ",");

	const items: Item[] = [];
	for (let index = 0; index < quantity; index++) {
		items.push({length, width, height, x, y, z});
	}

	return items;
}

function parseThree(compact: string, separator: string): [number, number, number] {
	const parts = compact.split(separator);
	if (parts.length !== 3) throw new Error(`'${compact}' must be three values separated by '${separator}'.`);
	return [Number(parts[0]), Number(parts[1]), Number(parts[2])];
}
