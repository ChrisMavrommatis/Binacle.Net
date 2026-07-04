using System.Numerics;
using Binacle.ViPaq.Abstractions;

namespace Binacle.ViPaq;

// An X/Y/Z position implementing IWithCoordinates<T>. Test-only: no public format accepts a standalone
// coordinate, so this exists purely to exercise BitSizeHelper.GetCoordinatesBitSize and the protocol writer in
// isolation. Lives in the test assembly so it stays off the library's public surface.
public class Coordinates<T> : IWithCoordinates<T>
	where T : struct, IBinaryInteger<T>
{
	public T X { get; set; }
	public T Y { get; set; }
	public T Z { get; set; }
}

// Factory so generic callers get T inferred (Coordinates.Create(value, value, value)) instead of writing the
// type argument on every `new Coordinates<T>`.
public static class Coordinates
{
	public static Coordinates<T> Create<T>(T x, T y, T z)
		where T : struct, IBinaryInteger<T>
		=> new() { X = x, Y = y, Z = z };
}
