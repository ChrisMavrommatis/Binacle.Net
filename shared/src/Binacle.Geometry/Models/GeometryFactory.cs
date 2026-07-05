using System.Numerics;

namespace Binacle.Geometry;

// Factory helpers so callers get T inferred (GeometryFactory.Dimensions(a, b, c)) instead of writing the
// type argument on every `new Dimensions<T>`.
public static class GeometryFactory
{
	public static Dimensions<T> Dimensions<T>(T length, T width, T height)
		where T : struct, IBinaryInteger<T>
		=> new() { Length = length, Width = width, Height = height };

	public static Coordinates<T> Coordinates<T>(T x, T y, T z)
		where T : struct, IBinaryInteger<T>
		=> new() { X = x, Y = y, Z = z };
}
