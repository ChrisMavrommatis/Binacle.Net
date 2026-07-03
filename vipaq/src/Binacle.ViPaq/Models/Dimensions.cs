using System.Numerics;
using Binacle.ViPaq.Abstractions;

namespace Binacle.ViPaq;

// A length/width/height measurement — the canonical dimensions-only implementation of IWithDimensions<T>.
public class Dimensions<T> : IWithDimensions<T>
	where T : struct,
	IBinaryInteger<T>,
	IComparable<T>,
	INumber<T>
{
	public T Length { get; set; }
	public T Width { get; set; }
	public T Height { get; set; }
}

// Factory so generic callers get T inferred (Dimensions.Create(value, value, value)) instead of writing the
// type argument on every `new Dimensions<T>`.
public static class Dimensions
{
	public static Dimensions<T> Create<T>(T length, T width, T height)
		where T : struct, IBinaryInteger<T>, IComparable<T>, INumber<T>
		=> new() { Length = length, Width = width, Height = height };
}
