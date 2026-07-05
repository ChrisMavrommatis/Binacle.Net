using System.Numerics;

namespace Binacle.CompactNotation;

// A concrete "(X,Y,Z)". What ParseCoordinates returns.
public class Coordinates<T> : IWithCoordinates<T>
	where T : struct, INumber<T>
{
	public T X { get; set; }
	public T Y { get; set; }
	public T Z { get; set; }
}
