using System.Numerics;

namespace Binacle.ViPaq.UnitTests;

// Builders for bins/items plus the serialize/deserialize assertions. Data is deterministic and
// curated (the per-field values come from BitSizeValues), not Bogus: ViPaq asserts on exact bytes,
// bit-size boundaries, and field order, so the values are load-bearing. Reintroduce Bogus only if a
// future test needs don't-care inputs (the way the Lib creation tests do).
public static class SerializationTestingFixture
{
	public static Bin<T> BuildBin<T>(BitSize size)
		where T : struct, IBinaryInteger<T> =>
		new()
		{
			Length = BitSizeValues.DistinctValue<T>(size, 0),
			Width = BitSizeValues.DistinctValue<T>(size, 1),
			Height = BitSizeValues.DistinctValue<T>(size, 2),
		};

	public static Item<T> BuildItem<T>(BitSize dimensionsSize, BitSize coordinatesSize)
		where T : struct, IBinaryInteger<T> =>
		new()
		{
			Length = BitSizeValues.DistinctValue<T>(dimensionsSize, 0),
			Width = BitSizeValues.DistinctValue<T>(dimensionsSize, 1),
			Height = BitSizeValues.DistinctValue<T>(dimensionsSize, 2),
			X = BitSizeValues.DistinctValue<T>(coordinatesSize, 3),
			Y = BitSizeValues.DistinctValue<T>(coordinatesSize, 4),
			Z = BitSizeValues.DistinctValue<T>(coordinatesSize, 5),
		};

	public static void AssertRoundTrips<T>(Bin<T> bin, IList<Item<T>> items)
		where T : struct, IBinaryInteger<T>
	{
		var data = ViPaqSerializer.Serialize<Bin<T>, Item<T>, T>(bin, items);
		var (resultBin, resultItems) = ViPaqSerializer.Deserialize<Bin<T>, Item<T>, T>(data);

		AssertSame(bin, items, resultBin, resultItems);
	}

	// Deserialize known bytes and check the bin and items match what we expect. Pins the decode path
	// against literal bytes, not just against the encoder.
	public static void AssertDeserializesTo<T>(byte[] data, Bin<T> bin, IList<Item<T>> items)
		where T : struct, IBinaryInteger<T>
	{
		var (resultBin, resultItems) = ViPaqSerializer.Deserialize<Bin<T>, Item<T>, T>(data);

		AssertSame(bin, items, resultBin, resultItems);
	}

	// Field-by-field compare. Bin and Item are plain classes (no value equality), so we check each
	// field; this also makes a wiring bug (one field read into another) show up as a clear mismatch.
	private static void AssertSame<T>(Bin<T> expectedBin, IList<Item<T>> expectedItems, Bin<T> actualBin, IList<Item<T>> actualItems)
		where T : struct, IBinaryInteger<T>
	{
		actualBin.Length.ShouldBe(expectedBin.Length);
		actualBin.Width.ShouldBe(expectedBin.Width);
		actualBin.Height.ShouldBe(expectedBin.Height);

		actualItems.Count.ShouldBe(expectedItems.Count);
		for (var i = 0; i < expectedItems.Count; i++)
		{
			actualItems[i].Length.ShouldBe(expectedItems[i].Length);
			actualItems[i].Width.ShouldBe(expectedItems[i].Width);
			actualItems[i].Height.ShouldBe(expectedItems[i].Height);
			actualItems[i].X.ShouldBe(expectedItems[i].X);
			actualItems[i].Y.ShouldBe(expectedItems[i].Y);
			actualItems[i].Z.ShouldBe(expectedItems[i].Z);
		}
	}
}
