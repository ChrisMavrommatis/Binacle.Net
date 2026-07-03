using System.Globalization;
using System.Numerics;
using System.Text;

namespace Binacle.CompactNotation;

// One text notation for geometry, in one place. Three blocks, fixed order, space-separated:
//   dimensions  "LxWxH"      split on 'x'
//   coordinates "(X,Y,Z)"    comma-separated inside parens
//   quantity    "[Q]"        one int inside brackets
// Valid entries: "LxWxH" | "LxWxH [Q]" | "LxWxH (X,Y,Z)" | "LxWxH (X,Y,Z) [Q]" | "(X,Y,Z)".
// Parsing is explicit — the caller usually knows the shape and calls the matching Parse method.
// When the shape is unknown, Detect picks the block. Parse is lenient about range (it just reads the
// integers); each consumer enforces its own limits.
public static class CompactNotation
{
	// --- parse (text -> model) ---

	// "LxWxH" -> Dimensions.
	public static Dimensions<T> ParseDimensions<T>(string compact)
		where T : struct, INumber<T>
	{
		var (length, width, height) = ParseThree<T>(compact.Trim(), 'x');
		return new Dimensions<T> { Length = length, Width = width, Height = height };
	}

	// "(X,Y,Z)" -> Coordinates. The parens are required.
	public static Coordinates<T> ParseCoordinates<T>(string compact)
		where T : struct, INumber<T>
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

	// "LxWxH (X,Y,Z)" -> one placed item. Coords are required; a "[Q]" repeat is a list concern
	// (use ParseItems).
	public static Item<T> ParseItem<T>(string compact)
		where T : struct, INumber<T>
	{
		if (compact.Contains('['))
			throw new FormatException($"Item '{compact}' carries a '[Q]' quantity — use ParseItems to expand it.");

		var (length, width, height, x, y, z) = ParseItemGeometry<T>(compact);
		return new Item<T> { Length = length, Width = width, Height = height, X = x, Y = y, Z = z };
	}

	// "LxWxH (X,Y,Z) [Q]" -> Q copies of the item (Q optional, default 1).
	public static IReadOnlyList<Item<T>> ParseItems<T>(string compact)
		where T : struct, INumber<T>
	{
		var quantity = 1;
		var body = compact.Trim();

		var bracket = body.IndexOf('[');
		if (bracket >= 0)
		{
			quantity = ParseQuantity(body[bracket..]);
			body = body[..bracket].Trim();
		}

		var (length, width, height, x, y, z) = ParseItemGeometry<T>(body);

		var items = new List<Item<T>>(quantity);
		for (var index = 0; index < quantity; index++)
			items.Add(new Item<T> { Length = length, Width = width, Height = height, X = x, Y = y, Z = z });

		return items;
	}

	// Flattens many compact item strings into one list; each may expand via its "[Q]" suffix.
	public static IReadOnlyList<Item<T>> ParseItems<T>(IEnumerable<string> compactItems)
		where T : struct, INumber<T>
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

	// --- format (model -> text) ---

	// Appends every block the value carries, in order. An object that is both dimensions and
	// coordinates formats as "LxWxH (X,Y,Z)"; add IWithQuantity and it gains " [Q]". T is the number
	// type the value's interfaces are closed over (int for the lib/API, long for vipaq).
	public static string Format<T>(object value)
		where T : struct, INumber<T>
	{
		var builder = new StringBuilder();

		if (value is IWithDimensions<T> dimensions)
			builder.Append(FormatDimensions(dimensions));

		if (value is IWithCoordinates<T> coordinates)
			AppendBlock(builder, FormatCoordinates(coordinates));

		if (value is IWithQuantity<T> quantity)
			AppendBlock(builder, FormatQuantity(quantity));

		if (builder.Length == 0)
			throw new ArgumentException($"'{value}' carries no compact-notation block.", nameof(value));

		return builder.ToString();
	}

	public static string FormatDimensions<T>(IWithDimensions<T> dimensions)
		where T : struct, INumber<T>
		=> $"{dimensions.Length}x{dimensions.Width}x{dimensions.Height}";

	public static string FormatCoordinates<T>(IWithCoordinates<T> coordinates)
		where T : struct, INumber<T>
		=> $"({coordinates.X},{coordinates.Y},{coordinates.Z})";

	public static string FormatQuantity<T>(IWithQuantity<T> quantity)
		where T : struct, INumber<T>
		=> $"[{quantity.Quantity}]";

	// --- helpers ---

	// "LxWxH (X,Y,Z)" -> the six numbers. Each part owns its separator ('x' vs ','), so a coordinate
	// is never read as a dimension.
	private static (T Length, T Width, T Height, T X, T Y, T Z) ParseItemGeometry<T>(string compact)
		where T : struct, INumber<T>
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
		where T : struct, INumber<T>
	{
		var parts = compact.Split(separator);
		if (parts.Length != 3)
			throw new FormatException($"'{compact}' must be three values separated by '{separator}'.");

		return (ParseNumber<T>(parts[0]), ParseNumber<T>(parts[1]), ParseNumber<T>(parts[2]));
	}

	private static T ParseNumber<T>(string value)
		where T : struct, INumber<T>
		=> T.Parse(value.Trim(), CultureInfo.InvariantCulture);

	private static void AppendBlock(StringBuilder builder, string block)
	{
		if (builder.Length > 0)
			builder.Append(' ');

		builder.Append(block);
	}
}
