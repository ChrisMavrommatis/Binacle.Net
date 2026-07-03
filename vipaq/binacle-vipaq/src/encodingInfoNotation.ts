import {BitSize, EncodingInfo, Version} from "./models";

// Text notation for the ViPaq encoding-info header only: "Version_Bin_ItemDim_ItemCoord", e.g.
// "Uncompressed_8_8_8" ("Compressed" = gzip). Wire-specific — it names EncodingInfo/BitSize/Version — so it
// stays in the vipaq mirror. The canonical geometry notation (dimensions/coordinates/items) lives in the
// shared binacle-compact-notation package, not here.

// "Version_Bin_ItemDim_ItemCoord" -> EncodingInfo. Version word then three widths.
export function parseEncodingInfo(compact: string): EncodingInfo {
	const parts = compact.split("_");
	if (parts.length !== 4) throw new Error(`EncodingInfo '${compact}' must be 'Version_Bin_ItemDim_ItemCoord'.`);
	return new EncodingInfo(parseVersion(parts[0]), parseWidth(parts[1]), parseWidth(parts[2]), parseWidth(parts[3]));
}

export function formatEncodingInfo(encodingInfo: EncodingInfo): string {
	return [
		formatVersion(encodingInfo.version),
		formatWidth(encodingInfo.binDimensionsBitSize),
		formatWidth(encodingInfo.itemDimensionsBitSize),
		formatWidth(encodingInfo.itemCoordinatesBitSize),
	].join("_");
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
