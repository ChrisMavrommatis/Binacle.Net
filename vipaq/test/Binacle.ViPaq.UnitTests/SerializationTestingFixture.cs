using System.Numerics;
using Binacle.ViPaq.Compression;

namespace Binacle.ViPaq.UnitTests;

// Builders for bins/items plus the encode/decode assertions. Data is deterministic and curated (the per-field
// values come from WidthValues), not Bogus: ViPaq asserts on exact bytes, width boundaries, and field order,
// so the values are load-bearing. Reintroduce Bogus only if a future test needs don't-care inputs (the way the
// Lib creation tests do).
//
// Encode and decode go through ProtocolEncoder, not ViPaqSerializer. The header — layout and all three widths —
// is an input to ProtocolEncoder, so a test can force a columnar or wider blob. That is the whole point:
// ViPaqSerializer chooses nothing yet, it always writes raw, row-major, narrowest, so a test driven through it
// can never reach the layout or width variants.
//
// Everything here is uncompressed: the encoder always gets the NoOp codec, so the body stays byte-for-byte
// readable (exact-byte pins depend on that). Compression is deferred (PROTOCOL.md §6) — no unit test writes or
// reads a compressed blob; ViPaqSerializer's refusal to read one is pinned separately in SerializationBehaviorTests.
internal static class SerializationTestingFixture
{
	// Builds a whole blob under a caller-chosen header. The header decides widths and layout; ProtocolEncoder
	// obeys it. Uncompressed only, so the codec is always NoOp.
	public static byte[] Encode<T>(Header header, Binacle.Geometry.Dimensions<T> bin, IReadOnlyList<Binacle.Geometry.Item<T>> items)
		where T : struct, IBinaryInteger<T>
		=> new ProtocolEncoder(new NoOpCodec())
			.Encode<Binacle.Geometry.Dimensions<T>, Binacle.Geometry.Item<T>, T>(header, bin, items);

	public static Binacle.Geometry.Dimensions<T> BuildBin<T>(Width size)
		where T : struct, IBinaryInteger<T> =>
		new()
		{
			Length = WidthValues.DistinctValue<T>(size, 0),
			Width = WidthValues.DistinctValue<T>(size, 1),
			Height = WidthValues.DistinctValue<T>(size, 2),
		};

	public static Binacle.Geometry.Item<T> BuildItem<T>(Width dimensionsSize, Width coordinatesSize)
		where T : struct, IBinaryInteger<T> =>
		new()
		{
			Length = WidthValues.DistinctValue<T>(dimensionsSize, 0),
			Width = WidthValues.DistinctValue<T>(dimensionsSize, 1),
			Height = WidthValues.DistinctValue<T>(dimensionsSize, 2),
			X = WidthValues.DistinctValue<T>(coordinatesSize, 3),
			Y = WidthValues.DistinctValue<T>(coordinatesSize, 4),
			Z = WidthValues.DistinctValue<T>(coordinatesSize, 5),
		};

	// Round-trips under a caller-chosen header, so the test controls compression, layout and widths.
	public static void AssertRoundTrips<T>(Header header, Binacle.Geometry.Dimensions<T> bin, IReadOnlyList<Binacle.Geometry.Item<T>> items)
		where T : struct, IBinaryInteger<T>
		=> AssertDeserializesTo(Encode(header, bin, items), bin, items);

	// Convenience for the width matrix, which only needs some valid header to round-trip: uses the library's
	// default (raw, row-major, narrowest) — the same header ViPaqSerializer would pick, but reached through
	// ProtocolEncoder like every other path here.
	public static void AssertRoundTrips<T>(Binacle.Geometry.Dimensions<T> bin, IReadOnlyList<Binacle.Geometry.Item<T>> items)
		where T : struct, IBinaryInteger<T>
		=> AssertRoundTrips(Header.Create<Binacle.Geometry.Dimensions<T>, Binacle.Geometry.Item<T>, T>(bin, items), bin, items);

	// Decode a whole blob (the two header bytes plus the body) and check the bin and items match. The header is
	// read off the wire; every blob here is uncompressed, so the codec is NoOp. Pins the decode path against
	// literal bytes, not just against the encoder.
	public static void AssertDeserializesTo<T>(byte[] data, Binacle.Geometry.Dimensions<T> bin, IReadOnlyList<Binacle.Geometry.Item<T>> items)
		where T : struct, IBinaryInteger<T>
	{
		var header = Header.FromBytes(data[0], data[1]);
		var (resultBin, resultItems) = new ProtocolEncoder(new NoOpCodec())
			.Decode<Binacle.Geometry.Dimensions<T>, Binacle.Geometry.Item<T>, T>(header, data[Header.ByteCount..]);

		AssertSame(bin, items, resultBin, resultItems);
	}

	// Field-by-field compare. Bin and Item are plain classes (no value equality), so we check each
	// field; this also makes a wiring bug (one field read into another) show up as a clear mismatch.
	private static void AssertSame<T>(Binacle.Geometry.Dimensions<T> expectedBin, IReadOnlyList<Binacle.Geometry.Item<T>> expectedItems, Binacle.Geometry.Dimensions<T> actualBin, IList<Binacle.Geometry.Item<T>> actualItems)
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
