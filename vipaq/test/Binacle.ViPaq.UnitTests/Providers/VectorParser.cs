namespace Binacle.ViPaq.UnitTests.Providers;

// Turns the shared vectors into typed values. The compact-geometry and encoding-info grammar is the library's
// CompactNotation (one grammar, shared with the interop generators); this class only owns the test-vector byte
// tokens and forwards the rest. Everything numeric is parsed as `long` — it holds the whole interoperable range
// [0, 2^53-1] exactly and is the natural pair for JS `number`.
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

	// Compact-geometry and encoding-info grammar lives in the library; these forward to it as `long`.
	public static Bin<long> ParseBin(string compact) 
		=> CompactNotation.ParseBin<long>(compact);

	public static Dimensions<long> ParseDimensions(string compact) 
		=> CompactNotation.ParseDimensions<long>(compact);

	public static Coordinates<long> ParseCoordinates(string compact) 
		=> CompactNotation.ParseCoordinates<long>(compact);

	public static List<Item<long>> ParseItems(string compact) 
		=> CompactNotation.ParseItems<long>(compact).ToList();

	public static List<Item<long>> ParseItems(IEnumerable<string> compactItems)
		=> CompactNotation.ParseItems<long>(compactItems).ToList();

	public static EncodingInfo ParseEncodingInfo(string compact) 
		=> CompactNotation.ParseEncodingInfo(compact);
}
