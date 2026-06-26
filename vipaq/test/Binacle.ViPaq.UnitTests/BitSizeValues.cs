using System.Numerics;

namespace Binacle.ViPaq.UnitTests;

// A distinct, deterministic value that fits inside a bit-size bucket. fieldIndex (0, 1, 2, ...)
// shifts each field to a different value, so a wiring bug (writing one field but reading another)
// shows up as a mismatch. Curated, not random: ViPaq asserts on exact bytes, bit-size boundaries,
// and field order, so the values are load-bearing. Shared by the serialization builders and the
// protocol-extension tests, so this "what value goes in each field" rule lives in one place.
public static class BitSizeValues
{
	public static T DistinctValue<T>(BitSize size, int fieldIndex)
		where T : struct, IBinaryInteger<T>, INumber<T>, IComparable<T>
	{
		var (baseValue, step) = size switch
		{
			BitSize.Eight => (10UL, 10UL),                         // 10, 20, 30, ...  (<= 255)
			BitSize.Sixteen => (300UL, 100UL),                     // 300, 400, 500, ...
			BitSize.ThirtyTwo => (70_000UL, 1_000UL),              // 70000, 71000, ...
			BitSize.SixtyFour => (5_000_000_000UL, 100_000_000UL), // ~5e9, +1e8 each
			_ => throw new ArgumentOutOfRangeException(nameof(size)),
		};
		return T.CreateChecked(baseValue + step * (ulong)fieldIndex);
	}
}
