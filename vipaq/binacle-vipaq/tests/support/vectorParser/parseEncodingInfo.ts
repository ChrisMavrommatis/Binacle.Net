import {BitSize, EncodingInfo, Version} from "../../../src/models";

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

// Ports C#: VectorParser.ParseEncodingInfo. "Compressed_8_8_16" -> EncodingInfo (Version word, three widths).
export function parseEncodingInfo(compact: string): EncodingInfo {
	const parts = compact.split("_");
	if (parts.length !== 4) throw new Error(`EncodingInfo '${compact}' must be 'Version_Bin_ItemDim_ItemCoord'.`);
	return new EncodingInfo(versionWords[parts[0]], widthWords[parts[1]], widthWords[parts[2]], widthWords[parts[3]]);
}
