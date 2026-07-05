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
