using System.Globalization;

namespace Binacle.ViPaq.Generators;

// Turns the shared vectors' compact strings into Bin/Item, following the same rules as the C# test
// VectorParser (see vipaq/test-vectors/README.md):
//   bin / dimensions — "LxWxH"  (split on 'x')
//   coordinates      — "X,Y,Z"  (split on ',')
//   item             — "LxWxH (X,Y,Z):Q"  where ":Q" repeats the item Q times (default 1)
// Each part owns its separator so a coordinate is never read as a dimension. A leading '-' is allowed
// so invalid-input scenarios can carry negatives. Everything parses as long. Kept minimal on purpose so
// the tool has no dependency on the test project's parser.
public static class CompactParser
{
	public static Bin ParseBin(string compact)
	{
		var (length, width, height) = ParseThree(compact, 'x');
		return new Bin { Length = length, Width = width, Height = height };
	}

	// "LxWxH (X,Y,Z):Q" -> Q copies of the item (Q optional, default 1). ':' is the quantity separator
	// (not '-') so '-' stays free for negative dims/coords.
	public static List<Item> ParseItems(string compact)
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

		var (length, width, height) = ParseThree(body[..space], 'x');

		var coordinatesText = body[(space + 1)..].Trim().TrimStart('(').TrimEnd(')');
		var (x, y, z) = ParseThree(coordinatesText, ',');

		var items = new List<Item>();
		for (var index = 0; index < quantity; index++)
			items.Add(new Item { Length = length, Width = width, Height = height, X = x, Y = y, Z = z });

		return items;
	}

	// Flattens many compact item strings into one list; each may expand via its ':Q' suffix.
	public static List<Item> ParseItems(IEnumerable<string> compactItems)
	{
		var result = new List<Item>();
		foreach (var compact in compactItems)
			result.AddRange(ParseItems(compact));

		return result;
	}

	// "A{separator}B{separator}C" -> three longs.
	private static (long First, long Second, long Third) ParseThree(string compact, char separator)
	{
		var parts = compact.Split(separator);
		if (parts.Length != 3)
			throw new FormatException($"'{compact}' must be three values separated by '{separator}'.");

		return (ParseLong(parts[0]), ParseLong(parts[1]), ParseLong(parts[2]));
	}

	private static long ParseLong(string value) => long.Parse(value, CultureInfo.InvariantCulture);
}
