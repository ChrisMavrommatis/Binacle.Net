using System.Numerics;
using System.Text;
using Binacle.Geometry;

namespace Binacle.CompactNotation;

// Formats geometry into the compact text notation — the inverse of CompactNotationParser. Format appends a
// block per interface the value carries: dimensions -> "LxWxH", coordinates -> " (X,Y,Z)",
// quantity -> " [Q]". It reads through the read-only interfaces, so both mutable and immutable objects work.
public static class CompactNotationFormatter
{
	// Appends every block the value carries, in order. An object that is both dimensions and coordinates
	// formats as "LxWxH (X,Y,Z)"; add a quantity and it gains " [Q]". T is the number type the value's
	// interfaces are closed over (int for the lib/API, long for vipaq).
	public static string Format<T>(object value)
		where T : struct, IBinaryInteger<T>
	{
		var builder = new StringBuilder();

		if (value is IWithReadOnlyDimensions<T> dimensions)
			builder.Append(FormatDimensions(dimensions));

		if (value is IWithReadOnlyCoordinates<T> coordinates)
			AppendBlock(builder, FormatCoordinates(coordinates));

		if (value is IWithReadOnlyQuantity<T> quantity)
			AppendBlock(builder, FormatQuantity(quantity));

		if (builder.Length == 0)
			throw new ArgumentException($"'{value}' carries no compact-notation block.", nameof(value));

		return builder.ToString();
	}

	public static string FormatDimensions<T>(IWithReadOnlyDimensions<T> dimensions)
		where T : struct, IBinaryInteger<T>
		=> $"{dimensions.Length}x{dimensions.Width}x{dimensions.Height}";

	public static string FormatCoordinates<T>(IWithReadOnlyCoordinates<T> coordinates)
		where T : struct, IBinaryInteger<T>
		=> $"({coordinates.X},{coordinates.Y},{coordinates.Z})";

	public static string FormatQuantity<T>(IWithReadOnlyQuantity<T> quantity)
		where T : struct, IBinaryInteger<T>
		=> $"[{quantity.Quantity}]";

	private static void AppendBlock(StringBuilder builder, string block)
	{
		if (builder.Length > 0)
			builder.Append(' ');

		builder.Append(block);
	}
}
