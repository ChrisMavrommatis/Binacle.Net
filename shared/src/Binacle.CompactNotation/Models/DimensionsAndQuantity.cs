using System.Numerics;
using Binacle.Geometry;

namespace Binacle.CompactNotation;

// A concrete "LxWxH [Q]" — dimensions plus how many of them. What ParseDimensionsAndQuantity returns.
// Unlike a placed Item (which ParseItems expands into copies), this keeps the count as a field. Call
// Flatten() when you want it expanded into that many standalone Dimensions.
public class DimensionsAndQuantity<T> : IWithDimensions<T>
	where T : struct, IBinaryInteger<T>
{
	public T Length { get; set; }
	public T Width { get; set; }
	public T Height { get; set; }
	public int Quantity { get; set; }

	// [Migrate-Review] No production consumer — only exercised by CompactNotation unit tests. Wire it to a real
	// caller or drop it (with its 2 tests). See .agents/plans/shared-geometry-extraction.md.
	// Expands into Quantity standalone dimensions (mirrors how ParseItems expands a placed item's "[Q]").
	public IReadOnlyList<Dimensions<T>> Flatten()
	{
		var result = new List<Dimensions<T>>(this.Quantity);
		for (var index = 0; index < this.Quantity; index++)
			result.Add(new Dimensions<T> { Length = this.Length, Width = this.Width, Height = this.Height });

		return result;
	}
}
