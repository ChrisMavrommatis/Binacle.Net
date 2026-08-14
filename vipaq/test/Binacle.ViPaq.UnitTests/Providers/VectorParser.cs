using Binacle.CompactNotation;

namespace Binacle.ViPaq.UnitTests.Providers;

// Turns the shared vectors into typed values. Geometry parsing forwards to the shared CompactNotationParser,
// one grammar for the whole repo, and returns the shared Binacle.Geometry models. Header notation stays
// vipaq-local because it needs Header/Width/Layout/Version.
//
// Everything numeric is parsed as `long`: it holds the whole interoperable range [0, 65_535] and is the natural
// pair for JS `number`.
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

	// Geometry via the shared notation; a bin is dimensions-only (the shared Dimensions model).
	public static Binacle.Geometry.Dimensions<long> ParseBin(string compact)
		=> CompactNotationParser.ParseDimensions<long>(compact);

	public static Dimensions<long> ParseDimensions(string compact)
		=> CompactNotationParser.ParseDimensions<long>(compact);

	public static Coordinates<long> ParseCoordinates(string compact)
		=> CompactNotationParser.ParseCoordinates<long>(compact);

	public static List<Binacle.Geometry.Item<long>> ParseItems(string compact)
		=> CompactNotationParser.ParseItems<long>(compact).ToList();

	public static List<Binacle.Geometry.Item<long>> ParseItems(IEnumerable<string> compactItems)
		=> CompactNotationParser.ParseItems<long>(compactItems).ToList();

	// Header notation is wire-specific, so it stays on vipaq's own notation.
	public static Header ParseHeader(string notation)
		=> HeaderNotation.Parse(notation);
}
