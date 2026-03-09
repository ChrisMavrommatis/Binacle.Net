using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.v4.Contracts.Pack;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

internal class PackCustomBinResponseExamples : IMultipleOpenApiExamplesProvider<PackBinResponse>
{
	public IEnumerable<IOpenApiExample<PackBinResponse>> GetExamples()
	{
		yield return OpenApiExample.Create(
			"fullypackedresponse",
			"Fully Packed Response",
			"Example response when all items fit into the bin and no items are left unpacked.",
			new PackBinResponse
			{
				Status = BinPackResultStatus.FullyPacked,
				Bin = Bin.From("custom_bin", 10, 40, 60),
				AlgorithmUsed = "FFD",
				PackedItems =
				[
					PackedBox.From("box_2", 10, 12, 15, 0, 0, 0),
					PackedBox.From("box_3", 10, 12, 15, 0, 12, 0),
					PackedBox.From("box_1", 2, 5, 10, 0, 0, 15),
					PackedBox.From("box_1", 2, 5, 10, 0, 24, 0),
				],
				UnpackedItems = [],
				PackedItemsVolumePercentage = 100,
				PackedBinVolumePercentage = 15.83m,
				ViPaqData = "AAQACig8CgwPAAAACgwPAAwAAgUKAAAPAgUKABgA"
			});

		yield return OpenApiExample.Create(
			"partiallypackedresponse",
			"Partially Packed Response",
			"Example response when some items fit into the bin but some items are left unpacked",
			new PackBinResponse()
			{
				Status = BinPackResultStatus.PartiallyPacked,
				Bin = Bin.From("custom_bin", 10, 40, 60),
				AlgorithmUsed = "FFD",
				PackedItems =
				[
					PackedBox.From("box_2", 10, 12, 15, 0, 0, 0),
					PackedBox.From("box_3", 10, 12, 15, 0, 12, 0),
				],
				UnpackedItems =
				[
					UnpackedBox.From("box_1", 2)
				],
				PackedItemsVolumePercentage = 79.37m,
				PackedBinVolumePercentage = 12.58m,
				ViPaqData = "AAQACig8CgwPAAAACgwPAAwAAgUKAAAPAgUKABgA"
			});

		yield return OpenApiExample.Create(
			"unpackedresponse",
			"Unpacked Response",
			"Example response when no items fit into the bin and all items are left unpacked",
			new PackBinResponse()
			{
				Status = BinPackResultStatus.NotPacked,
				Bin = Bin.From("custom_bin", 10, 40, 60),
				AlgorithmUsed = "FFD",
				PackedItems = [],
				UnpackedItems =[
					UnpackedBox.From("box_2", 1),
					UnpackedBox.From("box_3", 1),
					UnpackedBox.From("box_1", 2)
				],
				PackedItemsVolumePercentage = 0,
				PackedBinVolumePercentage = 0,
			});
	}
}
