using System.Globalization;
using System.Numerics;
using Binacle.Geometry;

namespace Binacle.CompactNotation;

// Parses the compact text notation for geometry (CompactNotationFormatter is the inverse). Three blocks,
// fixed order, space-separated:
//   dimensions  "LxWxH"     split on 'x'
//   coordinates "(X,Y,Z)"   comma-separated inside parens
//   quantity    "[Q]"       one int inside brackets
// Valid entries: "LxWxH" | "LxWxH [Q]" | "LxWxH (X,Y,Z)" | "LxWxH (X,Y,Z) [Q]" | "(X,Y,Z)".
// Parsing is explicit — the caller usually knows the shape and calls the matching method; Detect picks the
// block when the shape is unknown. Parse is lenient about range (it just reads the integers); each consumer
// enforces its own limits.
public static class CompactNotationParser
{
	// "LxWxH" -> Dimensions.
	public static Dimensions<T> ParseDimensions<T>(string compact)
		where T : struct, IBinaryInteger<T>
	{
		var (length, width, height) = ParseThree<T>(compact.Trim(), 'x');
		return new Dimensions<T> { Length = length, Width = width, Height = height };
	}

	// "(X,Y,Z)" -> Coordinates. The parens are required.
	public static Coordinates<T> ParseCoordinates<T>(string compact)
		where T : struct, IBinaryInteger<T>
	{
		var (x, y, z) = ParseThree<T>(StripParens(compact.Trim()), ',');
		return new Coordinates<T> { X = x, Y = y, Z = z };
	}

	// "[Q]" -> the quantity. The brackets are required.
	public static int ParseQuantity(string compact)
	{
		var body = compact.Trim();
		if (body.Length < 2 || body[0] != '[' || body[^1] != ']')
			throw new FormatException($"Quantity '{compact}' must be '[Q]'.");

		return int.Parse(body[1..^1], CultureInfo.InvariantCulture);
	}

	// "LxWxH" or "LxWxH [Q]" -> dimensions plus a count (default 1). Coordinates are not allowed here — a
	// placed item is ParseItem / ParseItems. Call Flatten() on the result to expand the count into copies.
	public static DimensionsAndQuantity<T> ParseDimensionsAndQuantity<T>(string compact)
		where T : struct, IBinaryInteger<T>
	{
		var (body, quantity) = SplitQuantity(compact);
		var dimensions = ParseDimensions<T>(body);
		return new DimensionsAndQuantity<T>
		{
			Length = dimensions.Length,
			Width = dimensions.Width,
			Height = dimensions.Height,
			Quantity = quantity,
		};
	}

	// "LxWxH (X,Y,Z)" -> one placed item. Coords are required; a "[Q]" repeat is a list concern
	// (use ParseItems).
	public static Item<T> ParseItem<T>(string compact)
		where T : struct, IBinaryInteger<T>
	{
		if (compact.Contains('['))
			throw new FormatException($"Item '{compact}' carries a '[Q]' quantity — use ParseItems to expand it.");

		var (length, width, height, x, y, z) = ParseItemGeometry<T>(compact);
		return new Item<T> { Length = length, Width = width, Height = height, X = x, Y = y, Z = z };
	}

	// "LxWxH (X,Y,Z) [Q]" -> Q copies of the item (Q optional, default 1).
	public static IReadOnlyList<Item<T>> ParseItems<T>(string compact)
		where T : struct, IBinaryInteger<T>
	{
		var (body, quantity) = SplitQuantity(compact);
		var (length, width, height, x, y, z) = ParseItemGeometry<T>(body);

		var items = new List<Item<T>>(quantity);
		for (var index = 0; index < quantity; index++)
			items.Add(new Item<T> { Length = length, Width = width, Height = height, X = x, Y = y, Z = z });

		return items;
	}

	// Flattens many compact item strings into one list; each may expand via its "[Q]" suffix.
	public static IReadOnlyList<Item<T>> ParseItems<T>(IEnumerable<string> compactItems)
		where T : struct, IBinaryInteger<T>
	{
		var result = new List<Item<T>>();
		foreach (var compact in compactItems)
			result.AddRange(ParseItems<T>(compact));

		return result;
	}

	// When the block is unknown, pick it from the leading token.
	public static CompactNotationKind Detect(string compact)
	{
		var text = compact.TrimStart();
		if (text.StartsWith('('))
			return CompactNotationKind.Coordinates;
		if (text.StartsWith('['))
			return CompactNotationKind.Quantity;
		if (text.Contains('x'))
			return CompactNotationKind.Dimensions;

		throw new FormatException($"'{compact}' is not a dimensions, coordinates, or quantity block.");
	}

	// --- helpers ---

	// Splits a trailing "[Q]" off a compact string. Returns the remaining body (trimmed) and the quantity,
	// defaulting to 1 when there is no "[Q]". Shared by ParseItems and ParseDimensionsAndQuantity.
	private static (string Body, int Quantity) SplitQuantity(string compact)
	{
		var body = compact.Trim();

		var bracket = body.IndexOf('[');
		if (bracket < 0)
			return (body, 1);

		return (body[..bracket].Trim(), ParseQuantity(body[bracket..]));
	}

	// "LxWxH (X,Y,Z)" -> the six numbers. Each part owns its separator ('x' vs ','), so a coordinate is never
	// read as a dimension.
	private static (T Length, T Width, T Height, T X, T Y, T Z) ParseItemGeometry<T>(string compact)
		where T : struct, IBinaryInteger<T>
	{
		var body = compact.Trim();
		var parenOpen = body.IndexOf('(');
		if (parenOpen < 0)
			throw new FormatException($"Item '{compact}' must be 'LxWxH (X,Y,Z)'.");

		var (length, width, height) = ParseThree<T>(body[..parenOpen].Trim(), 'x');
		var (x, y, z) = ParseThree<T>(StripParens(body[parenOpen..].Trim()), ',');

		return (length, width, height, x, y, z);
	}

	private static string StripParens(string text)
	{
		if (text.Length < 2 || text[0] != '(' || text[^1] != ')')
			throw new FormatException($"Coordinates '{text}' must be '(X,Y,Z)'.");

		return text[1..^1];
	}

	private static (T First, T Second, T Third) ParseThree<T>(string compact, char separator)
		where T : struct, IBinaryInteger<T>
	{
		var parts = compact.Split(separator);
		if (parts.Length != 3)
			throw new FormatException($"'{compact}' must be three values separated by '{separator}'.");

		return (ParseNumber<T>(parts[0]), ParseNumber<T>(parts[1]), ParseNumber<T>(parts[2]));
	}

	private static T ParseNumber<T>(string value)
		where T : struct, IBinaryInteger<T>
		=> T.Parse(value.Trim(), CultureInfo.InvariantCulture);
}
