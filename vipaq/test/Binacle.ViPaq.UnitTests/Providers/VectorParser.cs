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

	// Dimensions and bin are "LxWxH" (split on 'x'); coordinates are "X,Y,Z" (split on ','). Each parser
	// owns its separator so a coordinate is never read as a dimension. A leading '-' is allowed so
	// invalid-input vectors can carry negatives.
	public static Bin<long> ParseBin(string compact)
	{
		var dimensions = ParseDimensions(compact);
		return new Bin<long> { Length = dimensions.Length, Width = dimensions.Width, Height = dimensions.Height };
	}

	public static Dimensions<long> ParseDimensions(string compact)
	{
		var (length, width, height) = ParseThree(compact, 'x');
		return new Dimensions<long> { Length = length, Width = width, Height = height };
	}

	public static Coordinates<long> ParseCoordinates(string compact)
	{
		var (x, y, z) = ParseThree(compact, ',');
		return new Coordinates<long> { X = x, Y = y, Z = z };
	}

	// "LxWxH (X,Y,Z):Q" -> Q copies of the item (Q optional, default 1). ':' is the quantity separator
	// (not '-' as in shared/Binacle.TestsKernel) so '-' stays free for negative dims/coords.
	public static List<Item<long>> ParseItems(string compact)
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

		var dimensions = ParseDimensions(body[..space]);

		var coordinatesText = body[(space + 1)..].Trim().TrimStart('(').TrimEnd(')');
		var coordinates = ParseCoordinates(coordinatesText);

		var items = new List<Item<long>>();
		for (var i = 0; i < quantity; i++)
			items.Add(new Item<long>
			{
				Length = dimensions.Length,
				Width = dimensions.Width,
				Height = dimensions.Height,
				X = coordinates.X,
				Y = coordinates.Y,
				Z = coordinates.Z,
			});
		return items;
	}

	// Flattens many compact item strings into one list; each may expand via its ':Q' suffix.
	public static List<Item<long>> ParseItems(IEnumerable<string> compactItems)
	{
		var result = new List<Item<long>>();
		foreach (var compact in compactItems)
		{
			var items = ParseItems(compact);
			result.AddRange();
		}
			
		return result;
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

	// "A{separator}B{separator}C" -> three longs. Dimensions/bin split on 'x'; coordinates split on ','.
	private static (long First, long Second, long Third) ParseThree(string compact, char separator)
	{
		var parts = compact.Split(separator);
		if (parts.Length != 3)
			throw new FormatException($"'{compact}' must be three values separated by '{separator}'.");

		return (ParseLong(parts[0]), ParseLong(parts[1]), ParseLong(parts[2]));
	}

	private static long ParseLong(string value) => long.Parse(value, CultureInfo.InvariantCulture);
}
