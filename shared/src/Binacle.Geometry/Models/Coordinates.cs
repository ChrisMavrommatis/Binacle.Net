using System.Numerics;

namespace Binacle.Geometry;

// A concrete "(X,Y,Z)". Mutable, so it satisfies both the read-only and the settable interfaces.
public class Coordinates<T> : IWithCoordinates<T>
	where T : struct, IBinaryInteger<T>
{
	public T X { get; set; }
	public T Y { get; set; }
	public T Z { get; set; }
}

// Factory so generic callers get T inferred (Coordinates.Create(a, b, c)) instead of writing the
// type argument on every `new Coordinates<T>`.
public static class Coordinates
{
	public static Coordinates<T> Create<T>(T x, T y, T z)
		where T : struct, IBinaryInteger<T>
		=> new() { X = x, Y = y, Z = z };
}
