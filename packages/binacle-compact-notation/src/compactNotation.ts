import {Coordinates, CompactNotationKind, Dimensions, Item} from "./types";

// One text notation for geometry, mirroring the C# Binacle.CompactNotation. Three blocks, fixed order,
// space-separated:
//   dimensions  "LxWxH"     split on 'x'
//   coordinates "(X,Y,Z)"   comma-separated inside parens
//   quantity    "[Q]"       one int inside brackets
// Valid entries: "LxWxH" | "LxWxH [Q]" | "LxWxH (X,Y,Z)" | "LxWxH (X,Y,Z) [Q]" | "(X,Y,Z)".
// Parsing is explicit — the caller usually knows the shape and calls the matching parse function; detect
// picks the block when the shape is unknown. Numbers are plain JS `number`; the interoperable range is
// [0, 2^53-1]. Parse is lenient about range (it just reads the integers).

// --- parse (text -> shape) ---

export function parseDimensions(compact: string): Dimensions {
	const [length, width, height] = parseThree(compact.trim(), "x");
	return {length, width, height};
}

export function parseCoordinates(compact: string): Coordinates {
	const [x, y, z] = parseThree(stripParens(compact.trim()), ",");
	return {x, y, z};
}

export function parseQuantity(compact: string): number {
	const body = compact.trim();
	if (body.length < 2 || body[0] !== "[" || body[body.length - 1] !== "]")
		throw new Error(`Quantity '${compact}' must be '[Q]'.`);
	return parseNumber(body.slice(1, -1));
}

export function parseItem(compact: string): Item {
	if (compact.includes("["))
		throw new Error(`Item '${compact}' carries a '[Q]' quantity — use parseItems to expand it.`);
	return parseItemGeometry(compact);
}

// Expands an optional "[Q]" into Q copies. Accepts one string or many (flattened).
export function parseItems(compact: string | string[]): Item[] {
	if (Array.isArray(compact)) return compact.flatMap(parseItems);

	let quantity = 1;
	let body = compact.trim();
	const bracket = body.indexOf("[");
	if (bracket >= 0) {
		quantity = parseQuantity(body.slice(bracket));
		body = body.slice(0, bracket).trim();
	}

	const item = parseItemGeometry(body);
	const items: Item[] = [];
	for (let index = 0; index < quantity; index++) items.push({...item});
	return items;
}

export function detect(compact: string): CompactNotationKind {
	const text = compact.trimStart();
	if (text.startsWith("(")) return "coordinates";
	if (text.startsWith("[")) return "quantity";
	if (text.includes("x")) return "dimensions";
	throw new Error(`'${compact}' is not a dimensions, coordinates, or quantity block.`);
}

// --- format (shape -> text) ---

export function formatDimensions(dimensions: Dimensions): string {
	return `${dimensions.length}x${dimensions.width}x${dimensions.height}`;
}

export function formatCoordinates(coordinates: Coordinates): string {
	return `(${coordinates.x},${coordinates.y},${coordinates.z})`;
}

export function formatQuantity(quantity: number): string {
	return `[${quantity}]`;
}

// Appends every block the value carries, in order: dimensions, then coordinates, then quantity.
export function format(value: Partial<Item> & {quantity?: number}): string {
	const blocks: string[] = [];

	if (hasDimensions(value)) blocks.push(formatDimensions(value));
	if (hasCoordinates(value)) blocks.push(formatCoordinates(value));
	if (typeof value.quantity === "number") blocks.push(formatQuantity(value.quantity));

	if (blocks.length === 0) throw new Error("value carries no compact-notation block.");
	return blocks.join(" ");
}

// --- helpers ---

function parseItemGeometry(compact: string): Item {
	const body = compact.trim();
	const parenOpen = body.indexOf("(");
	if (parenOpen < 0) throw new Error(`Item '${compact}' must be 'LxWxH (X,Y,Z)'.`);

	const [length, width, height] = parseThree(body.slice(0, parenOpen).trim(), "x");
	const [x, y, z] = parseThree(stripParens(body.slice(parenOpen).trim()), ",");
	return {length, width, height, x, y, z};
}

function stripParens(text: string): string {
	if (text.length < 2 || text[0] !== "(" || text[text.length - 1] !== ")")
		throw new Error(`Coordinates '${text}' must be '(X,Y,Z)'.`);
	return text.slice(1, -1);
}

function parseThree(compact: string, separator: string): [number, number, number] {
	const parts = compact.split(separator);
	if (parts.length !== 3) throw new Error(`'${compact}' must be three values separated by '${separator}'.`);
	return [parseNumber(parts[0]), parseNumber(parts[1]), parseNumber(parts[2])];
}

// Reads one integer. Throws on empty or non-integer, matching C#'s long/int parse (so the two suites agree
// on malformed input).
function parseNumber(value: string): number {
	const trimmed = value.trim();
	const parsed = Number(trimmed);
	if (trimmed === "" || !Number.isInteger(parsed))
		throw new Error(`'${value}' is not an integer.`);
	return parsed;
}

function hasDimensions(value: Partial<Item>): value is Dimensions {
	return typeof value.length === "number" && typeof value.width === "number" && typeof value.height === "number";
}

function hasCoordinates(value: Partial<Item>): value is Coordinates {
	return typeof value.x === "number" && typeof value.y === "number" && typeof value.z === "number";
}
