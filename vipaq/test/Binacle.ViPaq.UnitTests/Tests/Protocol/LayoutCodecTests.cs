using Binacle.Geometry;
using Binacle.ViPaq.Compression;
using Binacle.ViPaq.Layouts;

namespace Binacle.ViPaq.UnitTests;

// Successor to the deleted ProtocolExtensionsTests and ProtocolExtensionsBehaviorTests. The old extensions
// wrote and read a dimensions/coordinates block field-by-field at a chosen width; that job now lives in the
// layout codecs, which lay the item fields out either row-major (each item whole: L W H X Y Z) or columnar
// (each field for every item). These drive both codecs through ProtocolEncoder with the NoOp codec (so the
// body stays readable) and prove three things: each layout round-trips, the two layouts differ on the wire but
// agree on the values, and an unknown layout code is rejected.
//
// Folded in from ProtocolExtensionsBehaviorTests: that test proved an unsupported BitSize was rejected. There
// is no BitSize any more; the equivalent guard is that LayoutCodecFactory rejects an unknown Layout code, which
// the two "Throws_For_Unknown_Layout" facts pin. Its over-ceiling read-reject case is gone with the 64-bit
// tier and now lives in the width-invalid vectors / SerializationBehaviorTests.
[Trait("Result Tests", "Ensures results are as expected")]
public class LayoutCodecTests
{
	private static readonly Dimensions<int> Bin = GeometryFactory.Dimensions(20, 20, 20);

	// Two items whose fields are all distinct, so row-major and columnar orderings produce different bytes.
	private static readonly Item<int>[] Items =
	[
		new() { Length = 1, Width = 2, Height = 3, X = 4, Y = 5, Z = 6 },
		new() { Length = 7, Width = 8, Height = 9, X = 10, Y = 11, Z = 12 },
	];

	// Layout is internal, so the boxed layout rides the theory data as object and is cast back inside the test.
	public static IEnumerable<object[]> Layouts =>
	[
		[Layout.RowMajor],
		[Layout.Columnar],
	];

	[Theory]
	[MemberData(nameof(Layouts))]
	public void Encode_Then_Decode_Round_Trips(object layoutValue)
	{
		var layout = (Layout)layoutValue;
		var encoder = new ProtocolEncoder(new NoOpCodec());
		var header = HeaderFor(layout);

		var blob = encoder.Encode<Dimensions<int>, Item<int>, int>(header, Bin, Items);
		var (bin, items) = encoder.Decode<Dimensions<int>, Item<int>, int>(header, blob[Header.ByteCount..]);

		bin.Length.ShouldBe(Bin.Length);
		bin.Width.ShouldBe(Bin.Width);
		bin.Height.ShouldBe(Bin.Height);

		items.Count.ShouldBe(Items.Length);
		for (var index = 0; index < Items.Length; index++)
		{
			items[index].Length.ShouldBe(Items[index].Length);
			items[index].Width.ShouldBe(Items[index].Width);
			items[index].Height.ShouldBe(Items[index].Height);
			items[index].X.ShouldBe(Items[index].X);
			items[index].Y.ShouldBe(Items[index].Y);
			items[index].Z.ShouldBe(Items[index].Z);
		}
	}

	// The layout bit really reorders the body: same values, same length, different bytes. Compares the body
	// (past the two header bytes, whose layout bit already differs) so the difference is the reordering itself.
	[Fact]
	public void The_Two_Layouts_Differ_On_The_Wire_But_Agree_On_Length()
	{
		var encoder = new ProtocolEncoder(new NoOpCodec());

		var rowMajor = encoder.Encode<Dimensions<int>, Item<int>, int>(HeaderFor(Layout.RowMajor), Bin, Items);
		var columnar = encoder.Encode<Dimensions<int>, Item<int>, int>(HeaderFor(Layout.Columnar), Bin, Items);

		var rowMajorBody = rowMajor[Header.ByteCount..];
		var columnarBody = columnar[Header.ByteCount..];

		columnarBody.Length.ShouldBe(rowMajorBody.Length);
		columnarBody.ShouldNotBe(rowMajorBody);
	}

	[Fact]
	public void CreateEncoder_Throws_For_Unknown_Layout()
		=> Should.Throw<ArgumentOutOfRangeException>(() => { LayoutCodecFactory<int>.CreateEncoder((Layout)99); });

	[Fact]
	public void CreateDecoder_Throws_For_Unknown_Layout()
		=> Should.Throw<ArgumentOutOfRangeException>(() => { LayoutCodecFactory<int>.CreateDecoder((Layout)99); });

	// Header.Create picks the narrowest widths for this data and defaults to RowMajor; `with` forces the layout.
	private static Header HeaderFor(Layout layout)
		=> Header.Create<Dimensions<int>, Item<int>, int>(Bin, Items) with { Layout = layout };
}
