namespace Binacle.ViPaq;

// The text form of a `Header`, used by the test vectors and the interop generators so a vector can name the
// exact header its bytes were produced under (PROTOCOL.md §6.1 requires this: byte comparison is only meaningful
// once the header is pinned).
//
// It stays in the library because it is wire-specific — it names `Header`, `Width`, `Layout` and `Version`,
// which the shared `Binacle.CompactNotation` leaf cannot hold. The geometry notation (dimensions, coordinates,
// items — `"10x10x10 (0,0,0)"`) is not here and never was; it lives in that leaf.
//
// ## The grammar
//
//   v{N}_{raw|comp}_{row|col}_{binW}_{itemDimW}_{itemCoordW}
//
// Six tokens, underscore-delimited, in wire order — it reads left to right exactly as the two header bytes lay
// out (`Version, Compressed, Layout | BinW, ItemDim, ItemCoord`). Examples:
//
//   v1_raw_row_8_8_8      uncompressed, row-major, all 8-bit (an empty or small pack)
//   v1_comp_col_16_8_16   compressed, columnar, 16-bit bin dims, 8-bit item dims, 16-bit item coords (Bischoff)
//
// Three calls the grammar makes, and why:
//
//   - **Version leads and is mandatory.** It is what makes positional parsing forward-safe: the header cannot
//     gain a field without a version bump, and a bump changes token 0, so `Parse` reads it first and rejects
//     anything but `v1` — the same stance as `Header.FromBytes`, which reads only `Version1`.
//   - **The compressed flag is a bare bit — `comp`, never `deflate` or `gzip`.** The header carries no codec
//     field; the codec is pinned by `Version` (PROTOCOL.md §6). Naming a codec here would imply the wire carries
//     one, which is the mistake §6 exists to prevent.
//   - **Widths are only `8` or `16`.** The reserved codes 2 and 3 never reach the wire (`Header.ToBytes`), so
//     they never reach the notation either; `Parse` and `Format` both reject them.
//
// A malformed notation string is a bad label, not a bad blob, so `Parse` throws `FormatException` — the .NET
// idiom for an unparseable string. `ViPaqFormatException` stays reserved for wire bytes.
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

	// Only Version1 has a spelling: it is the one version this implementation writes, and reserved codes never
	// reach the wire, so they have no notation either (mirrors `Header.ToBytes`).
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
