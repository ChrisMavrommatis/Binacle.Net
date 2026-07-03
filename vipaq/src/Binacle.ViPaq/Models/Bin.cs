using System.Numerics;
using Binacle.ViPaq.Abstractions;

namespace Binacle.ViPaq;

// A concrete bin — dimensions only. The canonical implementation of IWithDimensions<T> the library ships, so
// callers (tests, the interop generators, CompactNotation) don't each define their own.
public class Bin<T> : IWithDimensions<T>
	where T : struct,
	IBinaryInteger<T>,
	IComparable<T>,
	INumber<T>
{
	public T Length { get; set; }
	public T Width { get; set; }
	public T Height { get; set; }
}
