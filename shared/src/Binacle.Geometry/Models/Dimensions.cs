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
