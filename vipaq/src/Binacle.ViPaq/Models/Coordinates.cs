using System.Numerics;
using Binacle.ViPaq.Abstractions;

namespace Binacle.ViPaq;

// An X/Y/Z position — the canonical coordinates-only implementation of IWithCoordinates<T>.
public class Coordinates<T> : IWithCoordinates<T>
	where T : struct,
	IBinaryInteger<T>,
	IComparable<T>,
	INumber<T>
{
	public T X { get; set; }
	public T Y { get; set; }
	public T Z { get; set; }
}

// Factory so generic callers get T inferred (Coordinates.Create(value, value, value)) instead of writing the
// type argument on every `new Coordinates<T>`.
public static class Coordinates
{
	public static Coordinates<T> Create<T>(T x, T y, T z)
		where T : struct, IBinaryInteger<T>, IComparable<T>, INumber<T>
		=> new() { X = x, Y = y, Z = z };
}
