using System.Numerics;
using Binacle.Geometry;

namespace Binacle.ViPaq;

// A length/width/height measurement implementing IWithDimensions<T>. Test-only: the library never serializes a
// standalone dimensions, so this exists purely to exercise BitSizeHelper.GetDimensionsBitSize and the protocol
// writer in isolation. Lives in the test assembly so it stays off the library's public surface.
// [Migrate-Review] duplicates Binacle.Geometry.Dimensions<T> — drop this clone, keep only the Create factory
// (see .agents/plans/shared-geometry-extraction.md).
public class Dimensions<T> : IWithDimensions<T>
	where T : struct, IBinaryInteger<T>
{
	public T Length { get; set; }
	public T Width { get; set; }
	public T Height { get; set; }
}

// Factory so generic callers get T inferred (Dimensions.Create(value, value, value)) instead of writing the
// type argument on every `new Dimensions<T>`.
public static class Dimensions
{
	public static Dimensions<T> Create<T>(T length, T width, T height)
		where T : struct, IBinaryInteger<T>
		=> new() { Length = length, Width = width, Height = height };
}
