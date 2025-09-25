using Binacle.Net.v2.Models;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.v2.Responses.Examples;

internal class CustomPackResponseExamples : IMultipleOpenApiExamplesProvider<PackResponse>
{
	public IEnumerable<IOpenApiExample<PackResponse>> GetExamples()
	{
		yield return OpenApiExample.Create(
			"fullypackedresponse",
			"Fully Packed Response",
			"Fully Packed Response example.",
			PackResponse.Create(
				[
					new BinPackResult()
					{
						Bin = new Bin { ID = "custom_bin_1", Length = 10, Width = 40, Height = 60 },
						Result = BinPackResultStatus.FullyPacked,
						PackedItems =
						[
							new ResultBox
							{
								ID = "box_2",
								Dimensions = new Dimensions(10, 12, 15),
								Coordinates = new Coordinates(0, 0, 0)
							},
							new ResultBox
							{
								ID = "box_1",
								Dimensions = new Dimensions(2, 5, 10),
								Coordinates = new Coordinates(0, 12, 0)
							},
						],
						UnpackedItems = [],
						PackedItemsVolumePercentage = 100.00m,
						PackedBinVolumePercentage = 7.92m,
					},
					new BinPackResult()
					{
						Bin = new Bin { ID = "custom_bin_2", Length = 20, Width = 40, Height = 60 },
						Result = BinPackResultStatus.FullyPacked,
						PackedItems =
						[
							new ResultBox
							{
								ID = "box_2",
								Dimensions = new Dimensions(12, 15, 10),
								Coordinates = new Coordinates(0, 0, 0)
							},
							new ResultBox
							{
								ID = "box_1",
								Dimensions = new Dimensions(2, 5, 10),
								Coordinates = new Coordinates(12, 0, 0)
							},
						],
						UnpackedItems = [],
						PackedItemsVolumePercentage = 100.00m,
						PackedBinVolumePercentage = 3.96m,
					}
				]
			)
		);


		yield return OpenApiExample.Create(
			"partiallypackedresponse",
			"Partially Packed Response",
			"Partially Packed Response example.",
			PackResponse.Create(
				[
					new BinPackResult()
					{
						Bin = new Bin { ID = "custom_bin_2", Length = 20, Width = 40, Height = 60 },
						Result = BinPackResultStatus.PartiallyPacked,
						PackedItems =
						[
							new ResultBox
							{
								ID = "box_1",
								Dimensions = new Dimensions(2, 5, 10),
								Coordinates = new Coordinates(0, 0, 0)
							}
						],
						UnpackedItems =
						[
							new ResultBox
							{
								ID = "box_2",
								Dimensions = new Dimensions(12, 15, 20)
							}
						],
						PackedItemsVolumePercentage = 2.70m,
						PackedBinVolumePercentage = 0.42m,
					}
				]
			)
		);
	}
}
