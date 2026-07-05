using System.Numerics;

namespace Binacle.CompactNotation;

// A thing that has three dimensions. Read-only — the notation only reads to format.
// Objects that carry dimensions implement this so Format can find them (see CompactNotation.Format).
public interface IWithDimensions<T>
	where T : struct, INumber<T>
{
	T Length { get; }
	T Width { get; }
	T Height { get; }
}
