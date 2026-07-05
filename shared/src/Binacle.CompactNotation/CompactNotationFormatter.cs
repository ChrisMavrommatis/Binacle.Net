using System.Numerics;
using System.Text;

namespace Binacle.CompactNotation;

// Formats geometry into the compact text notation — the inverse of CompactNotationParser. Format appends a
// block per interface the value carries: IWithDimensions -> "LxWxH", IWithCoordinates -> " (X,Y,Z)",
// IWithQuantity -> " [Q]". The interfaces are read-only, so both mutable and immutable objects can be formatted.
public static class CompactNotationFormatter
{
	// Appends every block the value carries, in order. An object that is both dimensions and coordinates
	// formats as "LxWxH (X,Y,Z)"; add IWithQuantity and it gains " [Q]". T is the number type the value's
	// interfaces are closed over (int for the lib/API, long for vipaq).
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

	private static void AppendBlock(StringBuilder builder, string block)
	{
		if (builder.Length > 0)
			builder.Append(' ');

		builder.Append(block);
	}
}
