using System.Numerics;

namespace Binacle.CompactNotation;

// A thing that has an X,Y,Z position. Read-only — the notation only reads to format.
public interface IWithCoordinates<T>
	where T : struct, INumber<T>
{
	T X { get; }
	T Y { get; }
	T Z { get; }
}
