using System.Numerics;

namespace Binacle.Geometry;

// A concrete "LxWxH". Mutable, so it satisfies both the read-only and the settable interfaces.
public class Dimensions<T> : IWithDimensions<T>
	where T : struct, IBinaryInteger<T>
{
	public T Length { get; set; }
	public T Width { get; set; }
	public T Height { get; set; }
}

// Factory so generic callers get T inferred (Dimensions.Create(a, b, c)) instead of writing the
// type argument on every `new Dimensions<T>`.
public static class Dimensions
{
	public static Dimensions<T> Create<T>(T length, T width, T height)
		where T : struct, IBinaryInteger<T>
		=> new() { Length = length, Width = width, Height = height };
}
