using System.Numerics;

namespace Binacle.CompactNotation;

// A thing that has a quantity (how many). Read-only — the notation only reads to format.
public interface IWithQuantity<T>
	where T : struct, INumber<T>
{
	T Quantity { get; }
}
