using System.Diagnostics.CodeAnalysis;
using Version = Binacle.ViPaq.Version;

namespace Binacle.ViPaq;

// Text notation for the ViPaq encoding-info header only: "Version_Bin_ItemDim_ItemCoord", e.g.
// "Uncompressed_8_8_8" ("Compressed" = gzip). Wire-specific — it names EncodingInfo/BitSize/Version — so it
// stays in the library. The canonical geometry notation (dimensions/coordinates/items) lives in the shared
// Binacle.CompactNotation, not here; this type only knows the header string.
[Experimental("BINACLE_VIPAQ_COMPACT")]
public static class EncodingInfoNotation
{
	// "Version_Bin_ItemDim_ItemCoord" -> EncodingInfo. Version word then three widths.
	public static EncodingInfo ParseEncodingInfo(string compact)
	{
		var parts = compact.Split('_');
		if (parts.Length != 4)
			throw new FormatException($"EncodingInfo '{compact}' must be 'Version_Bin_ItemDim_ItemCoord'.");

		return new EncodingInfo
		{
			Version = ParseVersion(parts[0]),
			BinDimensionsBitSize = ParseWidth(parts[1]),
			ItemDimensionsBitSize = ParseWidth(parts[2]),
			ItemCoordinatesBitSize = ParseWidth(parts[3]),
		};
	}

	public static string FormatEncodingInfo(EncodingInfo encodingInfo)
		=> $"{FormatVersion(encodingInfo.Version)}_{FormatWidth(encodingInfo.BinDimensionsBitSize)}_" +
		   $"{FormatWidth(encodingInfo.ItemDimensionsBitSize)}_{FormatWidth(encodingInfo.ItemCoordinatesBitSize)}";

	private static Version ParseVersion(string word) => word switch
	{
		"Uncompressed" => Version.Uncompressed,
		"Compressed" => Version.CompressedGzip, // short word maps to the CompressedGzip enum
		"Reserved2" => Version.Reserved2,
		"Reserved3" => Version.Reserved3,
		_ => throw new FormatException($"Unknown version '{word}'."),
	};

	private static string FormatVersion(Version version) => version switch
	{
		Version.CompressedGzip => "Compressed",
		_ => version.ToString(),
	};

	private static BitSize ParseWidth(string word) => word switch
	{
		"8" => BitSize.Eight,
		"16" => BitSize.Sixteen,
		"32" => BitSize.ThirtyTwo,
		"64" => BitSize.SixtyFour,
		_ => throw new FormatException($"Unknown width '{word}'."),
	};

	private static int FormatWidth(BitSize bitSize) => bitSize switch
	{
		BitSize.Eight => 8,
		BitSize.Sixteen => 16,
		BitSize.ThirtyTwo => 32,
		BitSize.SixtyFour => 64,
		_ => throw new ArgumentOutOfRangeException(nameof(bitSize), bitSize, null),
	};
}
