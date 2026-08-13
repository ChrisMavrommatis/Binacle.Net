namespace Binacle.ViPaq;

// The text form of a `Header`, so a test vector can name the exact header its bytes were produced under
// (PROTOCOL.md §6.1: byte comparison only means something once the header is pinned).
//
// The grammar, six underscore-delimited tokens in wire order:
//
//   v{N}_{raw|comp}_{row|col}_{binW}_{itemDimW}_{itemCoordW}
//
//   v1_raw_row_8_8_8      uncompressed, row-major, all 8-bit (an empty or small pack)
//   v1_comp_col_16_8_16   compressed, columnar, 16-bit bin dims, 8-bit item dims, 16-bit item coords (Bischoff)
//
// Version leads and is mandatory: that is what makes positional parsing forward-safe, since the header cannot
// gain a field without a version bump. The compressed flag is a bare bit - never `deflate` or `gzip`, because
// the wire carries no codec field (PROTOCOL.md §6). Widths are only `8` or `16`; reserved codes 2 and 3 never
// reach the wire, so they have no notation.
//
// A malformed notation string is a bad label, not a bad blob, so `Parse` throws `FormatException`.
// `ViPaqFormatException` stays reserved for wire bytes.
internal static class HeaderNotation
{
	private const char Separator = '_';
	private const int TokenCount = 6;

	public static string Format(Header header)
	{
		return string.Join(
			Separator,
			FormatVersion(header.Version),
			header.Compressed ? "comp" : "raw",
			FormatLayout(header.Layout),
			FormatWidth(header.BinDimensionsWidth),
			FormatWidth(header.ItemDimensionsWidth),
			FormatWidth(header.ItemCoordinatesWidth)
			);
	}

	public static Header Parse(string notation)
	{
		ArgumentNullException.ThrowIfNull(notation);

		var tokens = notation.Split(Separator);
		if (tokens.Length != TokenCount)
		{
			throw new FormatException(
				$"Header notation must have {TokenCount} tokens separated by '{Separator}', got {tokens.Length}: '{notation}'"
				);
		}

		return new Header
		{
			Version = ParseVersion(tokens[0]),
			Compressed = ParseCompressed(tokens[1]),
			Layout = ParseLayout(tokens[2]),
			BinDimensionsWidth = ParseWidth(tokens[3], "bin dimensions"),
			ItemDimensionsWidth = ParseWidth(tokens[4], "item dimensions"),
			ItemCoordinatesWidth = ParseWidth(tokens[5], "item coordinates")
		};
	}

	// Only Version1 has a spelling - the one version written. Reserved codes never reach the wire.
	private static string FormatVersion(Version version)
	{
		return version switch
		{
			Version.Version1 => "v1",
			_ => throw new ArgumentOutOfRangeException(
				nameof(version),
				version,
				$"Only {Version.Version1} has a notation"
				)
		};
	}

	private static Version ParseVersion(string token)
	{
		return token switch
		{
			"v1" => Version.Version1,
			_ => throw new FormatException($"Unknown version token '{token}', this implementation reads 'v1'")
		};
	}

	private static bool ParseCompressed(string token)
	{
		return token switch
		{
			"comp" => true,
			"raw" => false,
			_ => throw new FormatException($"Unknown compression token '{token}', expected 'raw' or 'comp'")
		};
	}

	private static string FormatLayout(Layout layout)
	{
		return layout switch
		{
			Layout.RowMajor => "row",
			Layout.Columnar => "col",
			_ => throw new ArgumentOutOfRangeException(nameof(layout), layout, "Unknown layout")
		};
	}

	private static Layout ParseLayout(string token)
	{
		return token switch
		{
			"row" => Layout.RowMajor,
			"col" => Layout.Columnar,
			_ => throw new FormatException($"Unknown layout token '{token}', expected 'row' or 'col'")
		};
	}

	private static string FormatWidth(Width width)
	{
		return width switch
		{
			Width.Eight => "8",
			Width.Sixteen => "16",
			_ => throw new ArgumentOutOfRangeException(nameof(width), width, "Reserved width has no notation")
		};
	}

	private static Width ParseWidth(string token, string section)
	{
		return token switch
		{
			"8" => Width.Eight,
			"16" => Width.Sixteen,
			_ => throw new FormatException($"Unknown width token '{token}' for {section}, expected '8' or '16'")
		};
	}
}
