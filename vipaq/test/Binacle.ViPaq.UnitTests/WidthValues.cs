using System.Numerics;

namespace Binacle.ViPaq.UnitTests;

// A distinct, deterministic value inside a width bucket. fieldIndex shifts each field to a different value, so
// a wiring bug (writing one field but reading another) shows up as a mismatch. Curated, not random: ViPaq
// asserts on exact bytes, width boundaries and field order, so the values are load-bearing.
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
