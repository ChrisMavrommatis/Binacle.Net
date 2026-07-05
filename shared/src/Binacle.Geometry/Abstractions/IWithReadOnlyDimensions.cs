using System.Numerics;

namespace Binacle.Geometry;

// A thing that has three dimensions. Read-only — consumers that only read (e.g. formatting) use this.
public interface IWithReadOnlyDimensions<T>
	where T : struct, IBinaryInteger<T>
{
	T Length { get; }
	T Width { get; }
	T Height { get; }
}
