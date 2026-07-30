namespace Binacle.Net.v4.Contracts.Pack;

// The three outcomes every pack example is one of. Only the bin changes between them — the geometry, and so
// the layout and the percentages, does not.
internal static class PackExampleResponses
{
	internal static PackBinResponse FullyPacked(string binId)
		=> FullyPacked(
			ExampleData.SingleBin(binId)
		);

	internal static PackBinResponse FullyPacked(Bin bin)
	{
		var response = new PackBinResponse
		{
			Status = BinPackResultStatus.FullyPacked,
			Bin = bin,
			AlgorithmUsed = "FFD",
			PackedItems = ExampleData.AllItemsPacked(),
			UnpackedItems = [],
		};

		return response
			.WithVolumePercentages()
			.WithViPaqData();
	}

	internal static PackBinResponse PartiallyPacked(string binId)
		=> PartiallyPacked(
			ExampleData.SingleBin(binId)
		);

	internal static PackBinResponse PartiallyPacked(Bin bin)
	{
		var response = new PackBinResponse
		{
			Status = BinPackResultStatus.PartiallyPacked,
			Bin = bin,
			AlgorithmUsed = "FFD",
			PackedItems = ExampleData.SomeItemsPacked(),
			UnpackedItems = ExampleData.SomeItemsUnpacked(),
		};

		return response
			.WithVolumePercentages()
			.WithViPaqData();
	}

	internal static PackBinResponse NotPacked(string binId)
		=> NotPacked(
			ExampleData.SingleBin(binId)
		);

	internal static PackBinResponse NotPacked(Bin bin)
	{
		var response = new PackBinResponse
		{
			Status = BinPackResultStatus.NotPacked,
			Bin = bin,
			AlgorithmUsed = "FFD",
			PackedItems = [],
			UnpackedItems = ExampleData.AllItemsUnpacked(),
		};

		return response
			.WithVolumePercentages();
	}
}
