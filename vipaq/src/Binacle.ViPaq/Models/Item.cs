using System.Numerics;
using Binacle.Geometry;

namespace Binacle.ViPaq;

// A concrete item — dimensions plus a placement coordinate. The canonical implementation of
// IWithDimensions<T> + IWithCoordinates<T> the library ships, so callers don't each define their own.
// [Migrate-Review] geometry migration — verify interface wiring against Binacle.Geometry
// (see .agents/plans/shared-geometry-extraction.md).
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
