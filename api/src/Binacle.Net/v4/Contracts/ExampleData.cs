namespace Binacle.Net.v4.Contracts;

// The sample geometry every OpenAPI example is built from, in one place. Each member is a method, not a field:
// callers mutate what they get back (WithViPaqData and WithVolumePercentages both write to the instance they
// are handed), so every caller needs its own copy.
//
// The layouts here are what the algorithms actually produce for Items() in a 10x40x60 bin, which is why the
// packed dimensions are rotations of the requested ones.
internal static class ExampleData
{
	// Every example bin is this shape unless it is one of a set — see Bins.
	private const int binLength = 10;
	private const int binWidth = 40;
	private const int binHeight = 60;

	internal static List<Box> Items() =>
	[
		Box.From("box_1", 2, 5, 10, 2),
		Box.From("box_2", 12, 15, 10, 1),
		Box.From("box_3", 12, 10, 15, 1),
	];

	internal static Bin SingleBin(string id) => Bin.From(id, binLength, binWidth, binHeight);

	// Three bins of growing length, so the endpoints that choose one (smallest, best fit, compare) have
	// something to choose between.
	internal static List<Bin> Bins(string idPrefix) =>
	[
		Bin.From($"{idPrefix}_1", 10, binWidth, binHeight),
		Bin.From($"{idPrefix}_2", 20, binWidth, binHeight),
		Bin.From($"{idPrefix}_3", 30, binWidth, binHeight),
	];

	internal static List<PackedBox> AllItemsPacked() =>
	[
		PackedBox.From("box_2", 10, 12, 15, 0, 0, 0),
		PackedBox.From("box_3", 10, 12, 15, 0, 12, 0),
		PackedBox.From("box_1", 2, 5, 10, 0, 0, 15),
		PackedBox.From("box_1", 2, 5, 10, 0, 24, 0),
	];

	// The two large items placed, both box_1 left over.
	internal static List<PackedBox> SomeItemsPacked() =>
	[
		PackedBox.From("box_2", 10, 12, 15, 0, 0, 0),
		PackedBox.From("box_3", 10, 12, 15, 0, 12, 0),
	];

	internal static List<UnpackedBox> SomeItemsUnpacked() =>
	[
		UnpackedBox.From("box_1", 2),
	];

	internal static List<UnpackedBox> AllItemsUnpacked() =>
	[
		UnpackedBox.From("box_2", 1),
		UnpackedBox.From("box_3", 1),
		UnpackedBox.From("box_1", 2),
	];

	// Derived, not written down, for the same reason the ViPaq token is (see ViPaqExampleExtensions): a
	// hand-typed percentage drifts from the items beside it. The partially-packed examples had in fact drifted
	// — they claimed 79.37/12.58 for a layout the formula puts at 94.74/15.00.
	internal static T WithVolumePercentages<T>(this T response)
		where T : BinResponseBase
	{
		var itemsVolume = Items()
			.Sum(item => (long)item.Length * item.Width * item.Height * item.Quantity);
		var packedVolume = response.PackedItems?
			.Sum(box => (long)box.Length * box.Width * box.Height) ?? 0;
		var binVolume = (long)response.Bin.Length * response.Bin.Width * response.Bin.Height;

		response.PackedItemsVolumePercentage = AsPercentage(packedVolume, itemsVolume);
		response.PackedBinVolumePercentage = AsPercentage(packedVolume, binVolume);

		return response;
	}

	private static decimal AsPercentage(long value, long total)
		=> total == 0 ? 0 : Math.Round((decimal)value / total * 100, 2);
}
