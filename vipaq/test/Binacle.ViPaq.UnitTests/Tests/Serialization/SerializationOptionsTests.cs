namespace Binacle.ViPaq.UnitTests;

// ViPaqSerializer's opt-in options: compression and columnar layout. The default (raw, row-major) is pinned
// elsewhere; these prove the toggles round-trip and that the header records what the caller asked for.
[Trait("Behavioral Tests", "Ensures serializer options behave as expected")]
public class SerializationOptionsTests
{
	// 16-bit bin, so a coordinate can exceed 255; items are identical, so a large pack compresses well.
	private static readonly Binacle.Geometry.Dimensions<int> Bin = new() { Length = 1000, Width = 1000, Height = 1000 };

	private static List<Binacle.Geometry.Item<int>> RepetitiveItems(int count) =>
		Enumerable.Range(0, count)
			.Select(_ => new Binacle.Geometry.Item<int> { Length = 300, Width = 300, Height = 300, X = 0, Y = 0, Z = 0 })
			.ToList();

	// Every opt-in combination decodes back to the input — decode-to-input is the oracle (PROTOCOL.md §6.1).
	[Theory]
	[InlineData(false, Layout.RowMajor)]
	[InlineData(false, Layout.Columnar)]
	[InlineData(true, Layout.RowMajor)]
	[InlineData(true, Layout.Columnar)]
	public void Serialize_With_Options_RoundTrips(bool compress, Layout layout)
	{
		var expected = new BinContents<int>(Bin, RepetitiveItems(50));

		var blob = ViPaqSerializerTestingFixture.Serialize(expected, options =>
		{
			options.Compress = compress;
			options.Layout = layout;
		});
		var actual = ViPaqSerializerTestingFixture.Deserialize<int>(blob);

		BinContents.AssertSame(expected, actual);
	}

	// The chosen layout lands in the header even with compression off.
	[Fact]
	public void Serialize_Columnar_Sets_The_Layout_Bit()
	{
		var binContents = new BinContents<int>(Bin, RepetitiveItems(4));

		var blob = ViPaqSerializerTestingFixture.Serialize(binContents, options => options.Layout = Layout.Columnar);

		Header.FromBytes(blob[0], blob[1]).Layout.ShouldBe(Layout.Columnar);
	}

	// A large repetitive pack compresses, so the compressed blob is shorter and the header says so.
	[Fact]
	public void Serialize_Compresses_A_Large_Repetitive_Pack()
	{
		var expected = new BinContents<int>(Bin, RepetitiveItems(50));

		var raw = ViPaqSerializerTestingFixture.Serialize(expected);
		var compressed = ViPaqSerializerTestingFixture.Serialize(expected, options => options.Compress = true);
		var actual = ViPaqSerializerTestingFixture.Deserialize<int>(compressed);

		compressed.Length.ShouldBeLessThan(raw.Length);
		Header.FromBytes(compressed[0], compressed[1]).Compressed.ShouldBeTrue();
		BinContents.AssertSame(expected, actual);
	}
}
