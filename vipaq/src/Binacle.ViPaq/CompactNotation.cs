using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using Binacle.ViPaq.Abstractions;
using Version = Binacle.ViPaq.Version;

namespace Binacle.ViPaq;

// Experimental text notation for ViPaq inputs — a human-readable companion to the binary format
// (ViPaqSerializer, which does bytes). One grammar in one place instead of a copy per project:
//   dimensions / bin — "LxWxH"          (split on 'x')
//   coordinates      — "X,Y,Z"          (split on ',')
//   item             — "LxWxH (X,Y,Z)"  (ParseItems also expands an optional ":Q" repeat)
//   encoding info    — "Version_Bin_ItemDim_ItemCoord", e.g. "Uncompressed_8_8_8" ("Compressed" = gzip)
// Parse is lenient about range — it just reads the integers; ViPaqSerializer enforces [0, MaxInteger].
[Experimental("BINACLE_VIPAQ_COMPACT")]
public static class CompactNotation
{
	// --- parse (text -> model) ---

	public static Bin<T> ParseBin<T>(string compact)
		where T : struct, IBinaryInteger<T>, IComparable<T>, INumber<T>
	{
		var (length, width, height) = ParseThree<T>(compact, 'x');
		return new Bin<T> { Length = length, Width = width, Height = height };
	}

	// "LxWxH" -> Dimensions. Same split as ParseBin, but the dimensions-only type (used to test width picks).
	public static Dimensions<T> ParseDimensions<T>(string compact)
		where T : struct, IBinaryInteger<T>, IComparable<T>, INumber<T>
	{
		var (length, width, height) = ParseThree<T>(compact, 'x');
		return new Dimensions<T> { Length = length, Width = width, Height = height };
	}

	// "X,Y,Z" -> Coordinates (standalone, no parens; the parens belong to the item form).
	public static Coordinates<T> ParseCoordinates<T>(string compact)
		where T : struct, IBinaryInteger<T>, IComparable<T>, INumber<T>
	{
		var (x, y, z) = ParseThree<T>(compact, ',');
		return new Coordinates<T> { X = x, Y = y, Z = z };
	}

	// A single "LxWxH (X,Y,Z)" item. A ":Q" repeat is a list concern — use ParseItems for that.
	public static Item<T> ParseItem<T>(string compact)
		where T : struct, IBinaryInteger<T>, IComparable<T>, INumber<T>
	{
		if (compact.Contains(':'))
			throw new FormatException($"Item '{compact}' carries a ':Q' quantity — use ParseItems to expand it.");

		var (length, width, height, x, y, z) = ParseItemParts<T>(compact);
		return new Item<T> { Length = length, Width = width, Height = height, X = x, Y = y, Z = z };
	}

	// "LxWxH (X,Y,Z):Q" -> Q items (Q optional, default 1). ':' is the quantity separator, so '-' stays free
	// for negative dims/coords.
	public static IReadOnlyList<Item<T>> ParseItems<T>(string compact)
		where T : struct, IBinaryInteger<T>, IComparable<T>, INumber<T>
	{
		var quantity = 1;
		var body = compact;

		var colon = compact.IndexOf(':');
		if (colon >= 0)
		{
			body = compact[..colon];
			quantity = int.Parse(compact[(colon + 1)..], CultureInfo.InvariantCulture);
		}

		var (length, width, height, x, y, z) = ParseItemParts<T>(body);

		var items = new List<Item<T>>(quantity);
		for (var index = 0; index < quantity; index++)
			items.Add(new Item<T> { Length = length, Width = width, Height = height, X = x, Y = y, Z = z });

		return items;
	}

	// Flattens many compact item strings into one list; each may expand via its ':Q' suffix.
	public static IReadOnlyList<Item<T>> ParseItems<T>(IEnumerable<string> compactItems)
		where T : struct, IBinaryInteger<T>, IComparable<T>, INumber<T>
	{
		var result = new List<Item<T>>();
		foreach (var compact in compactItems)
			result.AddRange(ParseItems<T>(compact));

		return result;
	}

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

	// --- format (model -> text) ---

	public static string FormatDimensions<T>(IWithDimensions<T> dimensions)
		where T : struct, IBinaryInteger<T>, IComparable<T>, INumber<T>
		=> $"{dimensions.Length}x{dimensions.Width}x{dimensions.Height}";

	// Standalone "X,Y,Z" (no parens — the inverse of ParseCoordinates; the item form adds the parens).
	public static string FormatCoordinates<T>(IWithCoordinates<T> coordinates)
		where T : struct, IBinaryInteger<T>, IComparable<T>, INumber<T>
		=> $"{coordinates.X},{coordinates.Y},{coordinates.Z}";

	public static string FormatItem<T, TItem>(TItem item)
		where T : struct, IBinaryInteger<T>, IComparable<T>, INumber<T>
		where TItem : IWithDimensions<T>, IWithCoordinates<T>
		=> $"{item.Length}x{item.Width}x{item.Height} ({item.X},{item.Y},{item.Z})";

	public static string FormatEncodingInfo(EncodingInfo encodingInfo)
		=> $"{FormatVersion(encodingInfo.Version)}_{FormatWidth(encodingInfo.BinDimensionsBitSize)}_" +
		   $"{FormatWidth(encodingInfo.ItemDimensionsBitSize)}_{FormatWidth(encodingInfo.ItemCoordinatesBitSize)}";

	// --- helpers ---

	// "LxWxH (X,Y,Z)" -> the six numbers. Each part owns its separator ('x' vs ','), so a coordinate is never
	// read as a dimension.
	private static (T Length, T Width, T Height, T X, T Y, T Z) ParseItemParts<T>(string body)
		where T : struct, IBinaryInteger<T>, IComparable<T>, INumber<T>
	{
		var space = body.IndexOf(' ');
		if (space < 0)
			throw new FormatException($"Item '{body}' must be 'LxWxH (X,Y,Z)'.");

		var (length, width, height) = ParseThree<T>(body[..space], 'x');

		var coordinatesText = body[(space + 1)..].Trim().TrimStart('(').TrimEnd(')');
		var (x, y, z) = ParseThree<T>(coordinatesText, ',');

		return (length, width, height, x, y, z);
	}

	private static (T First, T Second, T Third) ParseThree<T>(string compact, char separator)
		where T : struct, IBinaryInteger<T>, IComparable<T>, INumber<T>
	{
		var parts = compact.Split(separator);
		if (parts.Length != 3)
			throw new FormatException($"'{compact}' must be three values separated by '{separator}'.");

		return (ParseNumber<T>(parts[0]), ParseNumber<T>(parts[1]), ParseNumber<T>(parts[2]));
	}

	private static T ParseNumber<T>(string value)
		where T : struct, IBinaryInteger<T>, IComparable<T>, INumber<T>
		=> T.Parse(value, CultureInfo.InvariantCulture);

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
