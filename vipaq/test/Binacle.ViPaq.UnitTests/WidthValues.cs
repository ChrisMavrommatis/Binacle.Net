using System.Numerics;

namespace Binacle.ViPaq.UnitTests;

// A distinct, deterministic value that fits inside a width bucket. fieldIndex (0, 1, 2, ...)
// shifts each field to a different value, so a wiring bug (writing one field but reading another)
// shows up as a mismatch. Curated, not random: ViPaq asserts on exact bytes, width boundaries,
// and field order, so the values are load-bearing. Shared by the serialization builders, so this
// "what value goes in each field" rule lives in one place. Only two widths exist now (Eight/Sixteen).
internal static class WidthValues
{
	public static T DistinctValue<T>(Width size, int fieldIndex)
		where T : struct, IBinaryInteger<T>
	{
		var (baseValue, step) = size switch
		{
			Width.Eight => (10UL, 10UL),                         // 10, 20, 30, ...  (<= 255)
			Width.Sixteen => (300UL, 100UL),                     // 300, 400, 500, ...
			_ => throw new ArgumentOutOfRangeException(nameof(size)),
		};
		return T.CreateChecked(baseValue + step * (ulong)fieldIndex);
	}
}
