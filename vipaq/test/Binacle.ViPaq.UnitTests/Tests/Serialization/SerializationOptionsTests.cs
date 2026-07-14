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

	private static byte[] Serialize(
		Binacle.Geometry.Dimensions<int> bin,
		IReadOnlyList<Binacle.Geometry.Item<int>> items,
		Action<ViPaqSerializationOptions>? configure = null)
		=> ViPaqSerializer.Serialize<Binacle.Geometry.Dimensions<int>, Binacle.Geometry.Item<int>, int>(bin, items, configure);

	private static void AssertRoundTrips(byte[] blob, Binacle.Geometry.Dimensions<int> bin, IReadOnlyList<Binacle.Geometry.Item<int>> items)
	{
		var (resultBin, resultItems) =
			ViPaqSerializer.Deserialize<Binacle.Geometry.Dimensions<int>, Binacle.Geometry.Item<int>, int>(blob);

		resultBin.Length.ShouldBe(bin.Length);
		resultBin.Width.ShouldBe(bin.Width);
		resultBin.Height.ShouldBe(bin.Height);

		resultItems.Count.ShouldBe(items.Count);
		for (var index = 0; index < items.Count; index++)
		{
			resultItems[index].Length.ShouldBe(items[index].Length);
			resultItems[index].Width.ShouldBe(items[index].Width);
			resultItems[index].Height.ShouldBe(items[index].Height);
			resultItems[index].X.ShouldBe(items[index].X);
			resultItems[index].Y.ShouldBe(items[index].Y);
			resultItems[index].Z.ShouldBe(items[index].Z);
		}
	}

	// Every opt-in combination decodes back to the input — decode-to-input is the oracle (PROTOCOL.md §6.1).
	[Theory]
	[InlineData(false, Layout.RowMajor)]
	[InlineData(false, Layout.Columnar)]
	[InlineData(true, Layout.RowMajor)]
	[InlineData(true, Layout.Columnar)]
	public void Serialize_With_Options_RoundTrips(bool compress, Layout layout)
	{
		var items = RepetitiveItems(50);
		var blob = Serialize(Bin, items, options =>
		{
			options.Compress = compress;
			options.Layout = layout;
		});

		AssertRoundTrips(blob, Bin, items);
	}

	// The chosen layout lands in the header even with compression off.
	[Fact]
	public void Serialize_Columnar_Sets_The_Layout_Bit()
	{
		var items = RepetitiveItems(4);
		var blob = Serialize(Bin, items, options => options.Layout = Layout.Columnar);

		Header.FromBytes(blob[0], blob[1]).Layout.ShouldBe(Layout.Columnar);
	}

	// A large repetitive pack compresses, so the compressed blob is shorter and the header says so.
	[Fact]
	public void Serialize_Compresses_A_Large_Repetitive_Pack()
	{
		var items = RepetitiveItems(50);

		var raw = Serialize(Bin, items);
		var compressed = Serialize(Bin, items, options => options.Compress = true);

		compressed.Length.ShouldBeLessThan(raw.Length);
		Header.FromBytes(compressed[0], compressed[1]).Compressed.ShouldBeTrue();
		AssertRoundTrips(compressed, Bin, items);
	}
}
