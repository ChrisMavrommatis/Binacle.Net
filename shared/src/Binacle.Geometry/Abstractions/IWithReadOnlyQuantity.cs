using System.Numerics;

namespace Binacle.Geometry;

// A thing that has a quantity (how many). Read-only — consumers that only read (e.g. formatting) use this.
public interface IWithReadOnlyQuantity<T>
	where T : struct, IBinaryInteger<T>
{
	T Quantity { get; }
}
