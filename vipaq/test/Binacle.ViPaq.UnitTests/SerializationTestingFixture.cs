using System.Numerics;
using Binacle.ViPaq.UnitTests.Models;

namespace Binacle.ViPaq.UnitTests;

// Deterministic builders + the serialize -> deserialize -> assert round trip. No randomness.
// Each of the six fields gets a distinct value inside its size bucket, so a wiring bug
// (writing one field but reading another) shows up as a mismatch.
public static class SerializationTestingFixture
{
	public static Bin<T> BuildBin<T>(BitSize size)
		where T : struct, IBinaryInteger<T>, INumber<T>, IComparable<T> =>
		new() { Length = Value<T>(size, 0), Width = Value<T>(size, 1), Height = Value<T>(size, 2) };

	public static Item<T> BuildItem<T>(BitSize dimensionsSize, BitSize coordinatesSize)
		where T : struct, IBinaryInteger<T>, INumber<T>, IComparable<T> =>
		new()
		{
			Length = Value<T>(dimensionsSize, 0),
			Width = Value<T>(dimensionsSize, 1),
			Height = Value<T>(dimensionsSize, 2),
			X = Value<T>(coordinatesSize, 3),
			Y = Value<T>(coordinatesSize, 4),
			Z = Value<T>(coordinatesSize, 5),
		};

	public static void AssertRoundTrips<T>(Bin<T> bin, IList<Item<T>> items)
		where T : struct, IBinaryInteger<T>, INumber<T>, IComparable<T>
	{
		var data = ViPaqSerializer.Serialize<Bin<T>, Item<T>, T>(bin, items);
		var (resultBin, resultItems) = ViPaqSerializer.Deserialize<Bin<T>, Item<T>, T>(data);

		resultBin.Length.ShouldBe(bin.Length);
		resultBin.Width.ShouldBe(bin.Width);
		resultBin.Height.ShouldBe(bin.Height);

		resultItems.Count.ShouldBe(items.Count);
		for (var i = 0; i < items.Count; i++)
		{
			resultItems[i].Length.ShouldBe(items[i].Length);
			resultItems[i].Width.ShouldBe(items[i].Width);
			resultItems[i].Height.ShouldBe(items[i].Height);
			resultItems[i].X.ShouldBe(items[i].X);
			resultItems[i].Y.ShouldBe(items[i].Y);
			resultItems[i].Z.ShouldBe(items[i].Z);
		}
	}

	// A distinct value inside the size bucket. slot (0..5) keeps the six fields different.
	private static T Value<T>(BitSize size, int slot)
		where T : struct, IBinaryInteger<T>, INumber<T>, IComparable<T>
	{
		var (baseValue, step) = size switch
		{
			BitSize.Eight => (10UL, 10UL),                         // 10..60   (<= 255)
			BitSize.Sixteen => (300UL, 100UL),                     // 300..800
			BitSize.ThirtyTwo => (70_000UL, 1_000UL),              // 70000..75000
			BitSize.SixtyFour => (5_000_000_000UL, 100_000_000UL), // ~5e9
			_ => throw new ArgumentOutOfRangeException(nameof(size)),
		};
		return T.CreateChecked(baseValue + step * (ulong)slot);
	}
}
