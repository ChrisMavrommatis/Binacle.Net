using System.Numerics;

namespace Binacle.Geometry;

// A thing that has an X,Y,Z position. Read-only — consumers that only read (e.g. formatting) use this.
public interface IWithReadOnlyCoordinates<T>
	where T : struct, IBinaryInteger<T>
{
	T X { get; }
	T Y { get; }
	T Z { get; }
}
