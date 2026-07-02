import {BitSize, EncodingInfo, Version} from "../src/models";

// Formats an EncodingInfo the same way encoding-info-bytes.json labels it (and the C# generator's
// ToLabel): "{Version}_{Bin}_{ItemDim}_{ItemCoord}", e.g. "Compressed_8_8_8". "Compressed" is the short
// word for gzip; widths are the numeric 8 / 16 / 32 / 64. Each artifact carries this string so the
// decode tests can pin byte 0 before trusting the round-trip.
export function toLabel(encodingInfo: EncodingInfo): string {
	return [
		versionLabel(encodingInfo.version),
		width(encodingInfo.binDimensionsBitSize),
		width(encodingInfo.itemDimensionsBitSize),
		width(encodingInfo.itemCoordinatesBitSize),
	].join("_");
}

function versionLabel(version: Version): string {
	switch (version) {
		case Version.Uncompressed: return "Uncompressed";
		case Version.CompressedGzip: return "Compressed";
		case Version.Reserved2: return "Reserved2";
		case Version.Reserved3: return "Reserved3";
	}
}

function width(bitSize: BitSize): number {
	switch (bitSize) {
		case BitSize.Eight: return 8;
		case BitSize.Sixteen: return 16;
		case BitSize.ThirtyTwo: return 32;
		case BitSize.SixtyFour: return 64;
	}
}
