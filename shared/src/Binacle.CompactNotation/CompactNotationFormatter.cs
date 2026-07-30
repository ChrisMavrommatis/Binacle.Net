using System.Numerics;
using Binacle.Geometry;

namespace Binacle.CompactNotation;

// Formats geometry into the compact text notation — the inverse of CompactNotationParser. Single-block primitives
// (FormatDimensions -> "LxWxH", FormatCoordinates -> "(X,Y,Z)", FormatQuantity -> "[Q]") plus the compile-guaranteed
// composites (FormatItem, FormatDimensionsAndQuantity). All read through the read-only interfaces.
public static class CompactNotationFormatter
{
	public static string FormatDimensions<T>(IWithReadOnlyDimensions<T> dimensions)
		where T : struct, IBinaryInteger<T>
		=> $"{dimensions.Length}x{dimensions.Width}x{dimensions.Height}";

	public static string FormatCoordinates<T>(IWithReadOnlyCoordinates<T> coordinates)
		where T : struct, IBinaryInteger<T>
		=> $"({coordinates.X},{coordinates.Y},{coordinates.Z})";

	public static string FormatQuantity<T>(IWithReadOnlyQuantity<T> quantity)
		where T : struct, IBinaryInteger<T>
		=> $"[{quantity.Quantity}]";

	// Concise, compile-guaranteed composites (mirror the parser's ParseItem / ParseDimensionsAndQuantity).
	// TValue is inferred from the argument, so callers write FormatItem(x) with no type args; the constraints
	// guarantee at compile time that x actually carries both blocks (a value missing one won't compile).

	// "LxWxH (X,Y,Z)" — dimensions plus a placement.
	public static string FormatItem<TValue>(TValue value)
		where TValue : IWithReadOnlyDimensions<int>, IWithReadOnlyCoordinates<int>
		=> $"{FormatDimensions(value)} {FormatCoordinates(value)}";

	// "LxWxH [Q]" — dimensions plus a count.
	public static string FormatDimensionsAndQuantity<TValue>(TValue value)
		where TValue : IWithReadOnlyDimensions<int>, IWithReadOnlyQuantity<int>
		=> $"{FormatDimensions(value)} {FormatQuantity(value)}";
}
