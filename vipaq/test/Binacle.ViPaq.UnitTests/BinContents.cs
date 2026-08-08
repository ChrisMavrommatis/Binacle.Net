using System.Numerics;

namespace Binacle.ViPaq.UnitTests;

// A bin and its items - the unit ViPaq serializes. Both sides of a comparison are one of these, so a test
// arranges the expected pack, acts to get the actual one, and hands the pair to AssertSame.
internal readonly record struct BinContents<T>(Binacle.Geometry.Dimensions<T> Bin, IReadOnlyList<Binacle.Geometry.Item<T>> Items)
	where T : struct, IBinaryInteger<T>;

// Everything both serialization paths share: the builders that make a pack, and the one comparison that
// checks two of them match. Neither belongs to a path - ProtocolTestingFixture and
// ViPaqSerializerTestingFixture both build inputs with these and both hand their result to AssertSame.
//
// Data is deterministic and curated (the per-field values come from WidthValues), not Bogus: ViPaq asserts on
// exact bytes, width boundaries, and field order, so the values are load-bearing. Reintroduce Bogus only if a
// future test needs don't-care inputs (the way the Lib creation tests do).
internal static class BinContents
{
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

	// Field-by-field compare. Bin and Item are plain classes (no value equality), so we check each
	// field; this also makes a wiring bug (one field read into another) show up as a clear mismatch.
	// Marked so the analyser knows this is where the checking happens - see AssertionMethodAttribute.
	[AssertionMethod]
	public static void AssertSame<T>(BinContents<T> expected, BinContents<T> actual)
		where T : struct, IBinaryInteger<T>
	{
		actual.Bin.Length.ShouldBe(expected.Bin.Length);
		actual.Bin.Width.ShouldBe(expected.Bin.Width);
		actual.Bin.Height.ShouldBe(expected.Bin.Height);

		actual.Items.Count.ShouldBe(expected.Items.Count);
		for (var i = 0; i < expected.Items.Count; i++)
		{
			actual.Items[i].Length.ShouldBe(expected.Items[i].Length);
			actual.Items[i].Width.ShouldBe(expected.Items[i].Width);
			actual.Items[i].Height.ShouldBe(expected.Items[i].Height);
			actual.Items[i].X.ShouldBe(expected.Items[i].X);
			actual.Items[i].Y.ShouldBe(expected.Items[i].Y);
			actual.Items[i].Z.ShouldBe(expected.Items[i].Z);
		}
	}
}
