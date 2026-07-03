using Binacle.CompactNotation;

namespace Binacle.ViPaq.UnitTests.Providers;

// Turns the shared vectors into typed values. Geometry parsing is the shared CompactNotationParser (one grammar
// for the whole repo); this class forwards to it and maps the result into vipaq's own Bin/Item so the
// serializer still gets its own types. (The shared Dimensions/Coordinates/Item are hidden by vipaq's own
// same-named models in the enclosing namespace, so only the parser type comes through the using.) Encoding-info
// notation stays vipaq-local (it needs EncodingInfo/BitSize/Version). It also owns the test-vector byte tokens.
// Everything numeric is parsed as `long` — it holds the whole interoperable range [0, 2^53-1] exactly and is
// the natural pair for JS `number`.
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

	public static byte[] ParseBytes(IEnumerable<string> tokens)
		=> tokens.Select(ParseByte).ToArray();

	// Geometry via the shared notation, mapped into vipaq's own models (a bin is dimensions-only).
	public static Bin<long> ParseBin(string compact)
	{
		var dimensions = CompactNotationParser.ParseDimensions<long>(compact);
		return new Bin<long> { Length = dimensions.Length, Width = dimensions.Width, Height = dimensions.Height };
	}

	public static Dimensions<long> ParseDimensions(string compact)
	{
		var dimensions = CompactNotationParser.ParseDimensions<long>(compact);
		return new Dimensions<long> { Length = dimensions.Length, Width = dimensions.Width, Height = dimensions.Height };
	}

	public static Coordinates<long> ParseCoordinates(string compact)
	{
		var coordinates = CompactNotationParser.ParseCoordinates<long>(compact);
		return new Coordinates<long> { X = coordinates.X, Y = coordinates.Y, Z = coordinates.Z };
	}

	public static List<Item<long>> ParseItems(string compact)
		=> CompactNotationParser.ParseItems<long>(compact).Select(ToItem).ToList();

	public static List<Item<long>> ParseItems(IEnumerable<string> compactItems)
		=> CompactNotationParser.ParseItems<long>(compactItems).Select(ToItem).ToList();

	// Encoding-info notation is wire-specific (EncodingInfo/BitSize/Version) — stays on vipaq's own notation.
	public static EncodingInfo ParseEncodingInfo(string compact)
		=> EncodingInfoNotation.ParseEncodingInfo(compact);

	private static Item<long> ToItem(Binacle.CompactNotation.Item<long> item)
		=> new() { Length = item.Length, Width = item.Width, Height = item.Height, X = item.X, Y = item.Y, Z = item.Z };
}
