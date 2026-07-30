namespace Binacle.Net.v4.Contracts.Fit;

// The three outcomes every fit example is one of. Only the bin changes between them — the geometry, and so
// the layout and the percentages, does not.
internal static class FitExampleResponses
{
	internal static FitBinResponse Fits(string binId)
		=> Fits(
			ExampleData.SingleBin(binId)
		);

	internal static FitBinResponse Fits(Bin bin)
	{
		var response = new FitBinResponse
		{
			Status = BinFitResultStatus.Fits,
			EarlyExitReason = BinFitEarlyExitReason.None,
			Bin = bin,
			AlgorithmUsed = "FFD",
			PackedItems = ExampleData.AllItemsPacked(),
			UnpackedItems = [],
		};

		return response
			.WithVolumePercentages()
			.WithViPaqData();
	}

	internal static FitBinResponse DoesNotFit(string binId)
		=> DoesNotFit(
			ExampleData.SingleBin(binId)
		);

	internal static FitBinResponse DoesNotFit(Bin bin)
	{
		var response = new FitBinResponse
		{
			Status = BinFitResultStatus.DoesNotFit,
			EarlyExitReason = BinFitEarlyExitReason.None,
			Bin = bin,
			AlgorithmUsed = "FFD",
			PackedItems = ExampleData.SomeItemsPacked(),
			UnpackedItems = ExampleData.SomeItemsUnpacked(),
		};

		return response
			.WithVolumePercentages()
			.WithViPaqData();
	}

	// Nothing is placed, so the bin is only ever the one the caller named. The oversized item is not one of
	// ExampleData.Items() — it is what triggers the exit.
	internal static FitBinResponse EarlyExit(string binId)
	{
		var response = new FitBinResponse
		{
			Status = BinFitResultStatus.EarlyExit,
			EarlyExitReason = BinFitEarlyExitReason.ContainerDimensionExceeded,
			Bin = ExampleData.SingleBin(binId),
			AlgorithmUsed = "FFD",
			PackedItems = [],
			UnpackedItems = [UnpackedBox.From("large_box", 1)],
			PackedItemsVolumePercentage = 0,
			PackedBinVolumePercentage = 0,
		};
		return response;
	}
}
