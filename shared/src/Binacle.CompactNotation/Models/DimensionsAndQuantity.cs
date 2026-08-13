using System.Numerics;
using Binacle.Geometry;

namespace Binacle.CompactNotation;

// A concrete "LxWxH [Q]", what ParseDimensionsAndQuantity returns. Unlike a placed Item, which ParseItems
// expands into copies, this keeps the count as a field; Flatten() expands it.
public class DimensionsAndQuantity<T> : IWithDimensions<T>
	where T : struct, IBinaryInteger<T>
{
	public T Length { get; set; }
	public T Width { get; set; }
	public T Height { get; set; }
	public int Quantity { get; set; }

	// Internal rather than public: no production consumer, only the CompactNotation unit tests.
	internal IReadOnlyList<Dimensions<T>> Flatten()
	{
		var result = new List<Dimensions<T>>(this.Quantity);
		for (var index = 0; index < this.Quantity; index++)
			result.Add(new Dimensions<T> { Length = this.Length, Width = this.Width, Height = this.Height });

		return result;
	}
}
