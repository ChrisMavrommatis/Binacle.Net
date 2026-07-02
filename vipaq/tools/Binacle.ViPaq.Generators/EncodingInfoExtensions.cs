using Version = Binacle.ViPaq.Version;

namespace Binacle.ViPaq.Generators;

public static class EncodingInfoExtensions
{
	// Formats an EncodingInfo the same way encoding-info-bytes.json labels it:
	//   "{Version}_{Bin}_{ItemDim}_{ItemCoord}"  e.g. "Compressed_8_8_8"
	// Version uses the short "Compressed" for gzip; widths are the numeric 8 / 16 / 32 / 64.
	// Each interop artifact carries this string so the decode tests can pin byte 0 before trusting the
	// round-trip — it catches a generator that silently stayed uncompressed (or picked the wrong widths).
	public static string ToLabel(this EncodingInfo encodingInfo) =>
		$"{encodingInfo.Version.ToLabel()}_{encodingInfo.BinDimensionsBitSize.ToWidth()}_" +
		$"{encodingInfo.ItemDimensionsBitSize.ToWidth()}_{encodingInfo.ItemCoordinatesBitSize.ToWidth()}";

	public static string ToLabel(this Version version) =>
		version == Version.CompressedGzip ? "Compressed" : version.ToString();

	public static int ToWidth(this BitSize bitSize) => bitSize switch
	{
		BitSize.Eight => 8,
		BitSize.Sixteen => 16,
		BitSize.ThirtyTwo => 32,
		BitSize.SixtyFour => 64,
		_ => throw new ArgumentOutOfRangeException(nameof(bitSize), bitSize, null),
	};
}
