using System.Globalization;
using Binacle.ViPaq.UnitTests.Models;
using Version = Binacle.ViPaq.Version;

namespace Binacle.ViPaq.UnitTests.Providers;

// Turns the shared vectors' compact strings and byte tokens into typed values. These are the same
// rules the README documents and the TypeScript loader must follow, so both sides read one input the
// same way. Everything is parsed as `long`: it holds the whole interoperable range [0, 2^53-1] exactly
// and is the natural pair for JS `number`.
internal static class VectorParser
{
	// "0x0A" (hex) or "0b00_01_00_00" (grouped binary) -> one byte. Underscores are separators.
	public static byte ParseByte(string token)
	{
		var normalized = token.Replace("_", "");
		if (normalized.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
			return Convert.ToByte(normalized[2..], 16);
		if (normalized.StartsWith("0b", StringComparison.OrdinalIgnoreCase))
			return Convert.ToByte(normalized[2..], 2);
		throw new FormatException($"Byte token '{token}' must start with 0x or 0b.");
	}

	public static byte[] ParseBytes(IEnumerable<string> tokens) => tokens.Select(ParseByte).ToArray();

	// "AxBxC" -> three longs. A leading '-' is allowed so invalid-input vectors can carry negatives.
	public static (long L, long W, long H) ParseTriple(string compact)
	{
		var parts = compact.Split('x');
		if (parts.Length != 3)
			throw new FormatException($"Triple '{compact}' must be 'AxBxC'.");

		return (ParseLong(parts[0]), ParseLong(parts[1]), ParseLong(parts[2]));
	}

	public static Bin<long> ParseBin(string compact)
	{
		var (l, w, h) = ParseTriple(compact);
		return new Bin<long> { Length = l, Width = w, Height = h };
	}

	public static Dimensions<long> ParseDimensions(string compact)
	{
		var (l, w, h) = ParseTriple(compact);
		return new Dimensions<long> { Length = l, Width = w, Height = h };
	}

	public static Coordinates<long> ParseCoordinates(string compact)
	{
		var (x, y, z) = ParseTriple(compact);
		return new Coordinates<long> { X = x, Y = y, Z = z };
	}

	// "LxWxH (X,Y,Z):Q" -> Q copies of the item (Q optional, default 1). ':' is the quantity separator
	// (not '-' as in shared/Binacle.TestsKernel) so '-' stays free for negative dims/coords.
	public static List<Item<long>> ParseItems(IEnumerable<string> compactItems)
	{
		var result = new List<Item<long>>();
		foreach (var compact in compactItems)
		{
			var (l, w, h, x, y, z, quantity) = ParseItemParts(compact);
			for (var i = 0; i < quantity; i++)
				result.Add(new Item<long> { Length = l, Width = w, Height = h, X = x, Y = y, Z = z });
		}
		return result;
	}

	private static (long L, long W, long H, long X, long Y, long Z, int Quantity) ParseItemParts(string compact)
	{
		var quantity = 1;
		var body = compact;

		var colon = compact.IndexOf(':');
		if (colon >= 0)
		{
			body = compact[..colon];
			quantity = int.Parse(compact[(colon + 1)..], CultureInfo.InvariantCulture);
		}

		var space = body.IndexOf(' ');
		if (space < 0)
			throw new FormatException($"Item '{compact}' must be 'LxWxH (X,Y,Z)'.");

		var (l, w, h) = ParseTriple(body[..space]);

		var coords = body[(space + 1)..].Trim().TrimStart('(').TrimEnd(')').Split(',');
		if (coords.Length != 3)
			throw new FormatException($"Item '{compact}' coordinates must be '(X,Y,Z)'.");

		return (l, w, h, ParseLong(coords[0]), ParseLong(coords[1]), ParseLong(coords[2]), quantity);
	}

	private static readonly Dictionary<string, Version> VersionWords = new()
	{
		["Uncompressed"] = Version.Uncompressed,
		["Compressed"] = Version.CompressedGzip, // short word maps to the CompressedGzip enum
		["Reserved2"] = Version.Reserved2,
		["Reserved3"] = Version.Reserved3,
	};

	private static readonly Dictionary<string, BitSize> WidthWords = new()
	{
		["8"] = BitSize.Eight,
		["16"] = BitSize.Sixteen,
		["32"] = BitSize.ThirtyTwo,
		["64"] = BitSize.SixtyFour,
	};

	// "Compressed_8_8_16" -> EncodingInfo. Version word then three widths; the documented exception to
	// "enum names, not numbers".
	public static EncodingInfo ParseEncodingInfo(string compact)
	{
		var parts = compact.Split('_');
		if (parts.Length != 4)
			throw new FormatException($"EncodingInfo '{compact}' must be 'Version_Bin_ItemDim_ItemCoord'.");

		return new EncodingInfo
		{
			Version = VersionWords[parts[0]],
			BinDimensionsBitSize = WidthWords[parts[1]],
			ItemDimensionsBitSize = WidthWords[parts[2]],
			ItemCoordinatesBitSize = WidthWords[parts[3]],
		};
	}

	private static long ParseLong(string value) => long.Parse(value, CultureInfo.InvariantCulture);
}
