using System.Numerics;
using Binacle.Geometry;

namespace Binacle.CompactNotation;

// A concrete "LxWxH (X,Y,Z)" — dimensions and a position. What ParseItem/ParseItems return.
// Quantity is not a field: it is expanded into copies by ParseItems, so each Item is one placed thing.
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
