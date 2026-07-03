import {BitSize, Coordinates, Dimensions, EncodingInfo, Version} from "./models";

// Experimental text notation for ViPaq inputs — a human-readable companion to the binary format
// (ViPaqSerializer). Mirrors the C# CompactNotation so both languages share one grammar in one place:
//   dimensions / bin — "LxWxH"          (split on 'x')
//   coordinates      — "X,Y,Z"          (split on ',')
//   item             — "LxWxH (X,Y,Z)"  (parseItems also expands an optional ":Q" repeat)
//   encoding info    — "Version_Bin_ItemDim_ItemCoord", e.g. "Uncompressed_8_8_8" ("Compressed" = gzip)
// Parse is lenient about range — it just reads the numbers; the serializer enforces [0, maxInteger].

type Item = Dimensions & Coordinates;

// --- parse (text -> model) ---

export function parseBin(compact: string): Dimensions {
	const [length, width, height] = parseThree(compact, "x");
	return {length, width, height};
}

// "LxWxH" -> Dimensions. Same split as parseBin, but the dimensions-only type (used to test width picks).
export function parseDimensions(compact: string): Dimensions {
	const [length, width, height] = parseThree(compact, "x");
	return {length, width, height};
}

// "X,Y,Z" -> Coordinates (standalone, no parens; the parens belong to the item form).
export function parseCoordinates(compact: string): Coordinates {
	const [x, y, z] = parseThree(compact, ",");
	return {x, y, z};
}

// A single "LxWxH (X,Y,Z)" item. A ":Q" repeat is a list concern — use parseItems for that.
export function parseItem(compact: string): Item {
	if (compact.includes(":")) throw new Error(`Item '${compact}' carries a ':Q' quantity — use parseItems.`);
	return parseItemParts(compact);
}

// "LxWxH (X,Y,Z):Q" -> Q items (Q optional, default 1). ':' is the quantity separator, so '-' stays free for
// negative dims/coords.
export function parseItems(compactItems: string[]): Item[] {
	const result: Item[] = [];
	for (const compact of compactItems) {
		let quantity = 1;
		let body = compact;
		const colon = compact.indexOf(":");
		if (colon >= 0) {
			body = compact.slice(0, colon);
			quantity = Number(compact.slice(colon + 1));
		}
		const parts = parseItemParts(body);
		for (let index = 0; index < quantity; index++) {
			result.push({...parts});
		}
	}
	return result;
}

// "Version_Bin_ItemDim_ItemCoord" -> EncodingInfo. Version word then three widths.
export function parseEncodingInfo(compact: string): EncodingInfo {
	const parts = compact.split("_");
	if (parts.length !== 4) throw new Error(`EncodingInfo '${compact}' must be 'Version_Bin_ItemDim_ItemCoord'.`);
	return new EncodingInfo(parseVersion(parts[0]), parseWidth(parts[1]), parseWidth(parts[2]), parseWidth(parts[3]));
}

// --- format (model -> text) ---

export function formatDimensions(dimensions: Dimensions): string {
	return `${dimensions.length}x${dimensions.width}x${dimensions.height}`;
}

// Standalone "X,Y,Z" (no parens — the inverse of parseCoordinates; the item form adds the parens).
export function formatCoordinates(coordinates: Coordinates): string {
	return `${coordinates.x},${coordinates.y},${coordinates.z}`;
}

export function formatItem(item: Item): string {
	return `${item.length}x${item.width}x${item.height} (${item.x},${item.y},${item.z})`;
}

export function formatEncodingInfo(encodingInfo: EncodingInfo): string {
	return [
		formatVersion(encodingInfo.version),
		formatWidth(encodingInfo.binDimensionsBitSize),
		formatWidth(encodingInfo.itemDimensionsBitSize),
		formatWidth(encodingInfo.itemCoordinatesBitSize),
	].join("_");
}

// --- helpers ---

function parseItemParts(body: string): Item {
	const space = body.indexOf(" ");
	if (space < 0) throw new Error(`Item '${body}' must be 'LxWxH (X,Y,Z)'.`);
	const [length, width, height] = parseThree(body.slice(0, space), "x");
	const coordinatesText = body.slice(space + 1).trim().replace(/^\(/, "").replace(/\)$/, "");
	const [x, y, z] = parseThree(coordinatesText, ",");
	return {length, width, height, x, y, z};
}

function parseThree(compact: string, separator: string): [number, number, number] {
	const parts = compact.split(separator);
	if (parts.length !== 3) throw new Error(`'${compact}' must be three values separated by '${separator}'.`);
	return [Number(parts[0]), Number(parts[1]), Number(parts[2])];
}

const versionWords: Record<string, Version> = {
	Uncompressed: Version.Uncompressed,
	Compressed: Version.CompressedGzip, // short word maps to the CompressedGzip enum
	Reserved2: Version.Reserved2,
	Reserved3: Version.Reserved3,
};

const widthWords: Record<string, BitSize> = {
	"8": BitSize.Eight,
	"16": BitSize.Sixteen,
	"32": BitSize.ThirtyTwo,
	"64": BitSize.SixtyFour,
};

function parseVersion(word: string): Version {
	const version = versionWords[word];
	if (version === undefined) throw new Error(`Unknown version '${word}'.`);
	return version;
}

function parseWidth(word: string): BitSize {
	const width = widthWords[word];
	if (width === undefined) throw new Error(`Unknown width '${word}'.`);
	return width;
}

function formatVersion(version: Version): string {
	switch (version) {
		case Version.Uncompressed: return "Uncompressed";
		case Version.CompressedGzip: return "Compressed";
		case Version.Reserved2: return "Reserved2";
		case Version.Reserved3: return "Reserved3";
	}
}

function formatWidth(bitSize: BitSize): number {
	switch (bitSize) {
		case BitSize.Eight: return 8;
		case BitSize.Sixteen: return 16;
		case BitSize.ThirtyTwo: return 32;
		case BitSize.SixtyFour: return 64;
	}
}
