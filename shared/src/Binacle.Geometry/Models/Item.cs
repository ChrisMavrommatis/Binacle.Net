using System.Numerics;

namespace Binacle.Geometry;

// A concrete "LxWxH (X,Y,Z)" — dimensions plus a placement. The shared placed-item model used across the
// notation, vipaq, and any caller that needs a ready-made mutable item. Mutable so it satisfies both the
// read-only and settable interfaces (and vipaq deserialize, which needs setters + a parameterless ctor).
public class Item<T> : IWithDimensions<T>, IWithCoordinates<T>
	where T : struct, IBinaryInteger<T>
{
	public T Length { get; set; }
	public T Width { get; set; }
	public T Height { get; set; }
	public T X { get; set; }
	public T Y { get; set; }
	public T Z { get; set; }
}
