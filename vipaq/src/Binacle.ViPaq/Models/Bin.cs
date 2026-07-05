using System.Numerics;
using Binacle.Geometry;

namespace Binacle.ViPaq;

// A concrete bin — dimensions only. The canonical implementation of IWithDimensions<T> the library ships, so
// callers (tests, the interop generators) don't each define their own.
// [Migrate-Review] geometry migration — verify interface wiring against Binacle.Geometry
// (see .agents/plans/shared-geometry-extraction.md).
public class Bin<T> : IWithDimensions<T>
	where T : struct, IBinaryInteger<T>
{
	public T Length { get; set; }
	public T Width { get; set; }
	public T Height { get; set; }
}
