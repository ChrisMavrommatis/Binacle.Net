using System.Numerics;
using Binacle.Geometry;

namespace Binacle.CompactNotation;

// Formats geometry into the compact text notation, the inverse of CompactNotationParser. Single-block
// primitives ("LxWxH", "(X,Y,Z)", "[Q]") plus the two composites below.
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

	// Mirrors the parser's ParseItem / ParseDimensionsAndQuantity. TValue is inferred, and the constraints
	// reject a value missing a block at compile time.

	// "LxWxH (X,Y,Z)" — dimensions plus a placement.
	public static string FormatItem<TValue>(TValue value)
		where TValue : IWithReadOnlyDimensions<int>, IWithReadOnlyCoordinates<int>
		=> $"{FormatDimensions(value)} {FormatCoordinates(value)}";

	// "LxWxH [Q]" — dimensions plus a count.
	public static string FormatDimensionsAndQuantity<TValue>(TValue value)
		where TValue : IWithReadOnlyDimensions<int>, IWithReadOnlyQuantity<int>
		=> $"{FormatDimensions(value)} {FormatQuantity(value)}";
}
