namespace Binacle.Net.v3.Contracts;

// The sample geometry every OpenAPI example is built from, in one place — the v3 mirror of v4's ExampleData.
// Percentages are derived (WithVolumePercentages), not hand-typed, so a documentation number can never drift
// from the items beside it (the old hardcoded values had); the ViPaq token is derived the same way (see
// ViPaqExampleExtensions). The packed layouts are what the algorithms actually produce for Items() in a
// 10x40x60 bin, which is why the packed dimensions are rotations of the requested ones.
internal static class ExampleData
{
	private const int binLength = 10;
	private const int binWidth = 40;
	private const int binHeight = 60;

	internal static List<Box> Items() =>
	[
		new() { ID = "box_1", Quantity = 2, Length = 2, Width = 5, Height = 10 },
		new() { ID = "box_2", Quantity = 1, Length = 12, Width = 15, Height = 10 },
		new() { ID = "box_3", Quantity = 1, Length = 12, Width = 10, Height = 15 },
	];

	internal static Bin SingleBin(string id) =>
		new() { ID = id, Length = binLength, Width = binWidth, Height = binHeight };

	// Two bins of growing length, so the custom endpoints have more than one to report on.
	internal static List<Bin> Bins(string idPrefix) =>
	[
		new() { ID = $"{idPrefix}_1", Length = 10, Width = binWidth, Height = binHeight },
		new() { ID = $"{idPrefix}_2", Length = 20, Width = binWidth, Height = binHeight },
	];

	internal static List<PackedBox> AllItemsPacked() =>
	[
		new() { ID = "box_2", Length = 10, Width = 12, Height = 15, X = 0, Y = 0, Z = 0 },
		new() { ID = "box_3", Length = 10, Width = 12, Height = 15, X = 0, Y = 12, Z = 0 },
		new() { ID = "box_1", Length = 2, Width = 5, Height = 10, X = 0, Y = 0, Z = 15 },
		new() { ID = "box_1", Length = 2, Width = 5, Height = 10, X = 0, Y = 24, Z = 0 },
	];

	// The two large items placed, both box_1 left over.
	internal static List<PackedBox> SomeItemsPacked() =>
	[
		new() { ID = "box_2", Length = 10, Width = 12, Height = 15, X = 0, Y = 0, Z = 0 },
		new() { ID = "box_3", Length = 10, Width = 12, Height = 15, X = 0, Y = 12, Z = 0 },
	];

	internal static List<UnpackedBox> SomeItemsUnpacked() =>
	[
		new() { ID = "box_1", Quantity = 2 },
	];

	// Fit uses its own item type, without placement coordinates.
	internal static List<FittedBox> AllItemsFitted() =>
	[
		new() { ID = "box_2", Length = 10, Width = 12, Height = 15 },
		new() { ID = "box_3", Length = 10, Width = 12, Height = 15 },
		new() { ID = "box_1", Length = 2, Width = 5, Height = 10 },
		new() { ID = "box_1", Length = 2, Width = 5, Height = 10 },
	];

	internal static List<FittedBox> SomeItemsFitted() =>
	[
		new() { ID = "box_2", Length = 10, Width = 12, Height = 15 },
		new() { ID = "box_3", Length = 10, Width = 12, Height = 15 },
	];

	internal static List<UnfittedBox> SomeItemsUnfitted() =>
	[
		new() { ID = "box_1", Quantity = 2 },
	];

	// An item larger than any example bin — what triggers an early-fail check. Not one of Items().
	internal static List<UnfittedBox> OversizedItemUnfitted() =>
	[
		new() { ID = "large_box", Quantity = 1 },
	];

	// Factories rather than a mutate-after extension because the percentages are required members: deriving them
	// here means a hand-typed number can never drift from the items beside it, the same reason the ViPaq token is
	// derived (see ViPaqExampleExtensions).
	internal static BinPackResult PackedResult(
		Bin bin,
		BinPackResultStatus status,
		List<PackedBox> packedItems,
		List<UnpackedBox> unpackedItems
	)
	{
		var packedVolume = packedItems.Sum(box => (long)box.Length * box.Width * box.Height);
		return new BinPackResult
		{
			Bin = bin,
			Result = status,
			PackedItems = packedItems,
			UnpackedItems = unpackedItems,
			PackedItemsVolumePercentage = AsPercentage(packedVolume, ItemsVolume()),
			PackedBinVolumePercentage = AsPercentage(packedVolume, BinVolume(bin)),
		};
	}

	internal static BinFitResult FittedResult(
		Bin bin,
		BinFitResultStatus status,
		List<FittedBox> fittedItems,
		List<UnfittedBox> unfittedItems
	)
	{
		var fittedVolume = fittedItems.Sum(box => (long)box.Length * box.Width * box.Height);
		return new BinFitResult
		{
			Bin = bin,
			Result = status,
			FittedItems = fittedItems,
			UnfittedItems = unfittedItems,
			FittedItemsVolumePercentage = AsPercentage(fittedVolume, ItemsVolume()),
			FittedBinVolumePercentage = AsPercentage(fittedVolume, BinVolume(bin)),
		};
	}

	private static long ItemsVolume() =>
		Items().Sum(item => (long)item.Length * item.Width * item.Height * item.Quantity);

	private static long BinVolume(Bin bin) => (long)bin.Length * bin.Width * bin.Height;

	private static decimal AsPercentage(long value, long total)
		=> total == 0 ? 0 : Math.Round((decimal)value / total * 100, 2);
}
