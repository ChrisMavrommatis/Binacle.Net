using Binacle.Net.Api.v3.Models;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.Api.v3.Responses.Examples;

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
							new PackedBox()
							{
								ID = "box_2",
								Length = 10,
								Width = 12,
								Height = 15,
								X = 0,
								Y = 0,
								Z = 0
							},
							new PackedBox
							{
								ID = "box_1",
								Length = 2,
								Width = 5,
								Height = 10,
								X = 0,
								Y = 12,
								Z = 0
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
							new PackedBox()
							{
								ID = "box_2",
								Length = 12,
								Width = 15,
								Height = 10,
								X = 0,
								Y = 0,
								Z = 0
							},
							new PackedBox()
							{
								ID = "box_1",
								Length = 2,
								Width = 5,
								Height = 10,
								X = 12,
								Y = 0,
								Z = 0
							}
						],
						UnpackedItems = [],
						PackedItemsVolumePercentage = 100.00m,
						PackedBinVolumePercentage = 3.96m,
					}
				]
			));


		yield return OpenApiExample.Create(
			"partiallypackedresponse",
			"Partially Packed Response",
			"Partially Packed Response example.",
			PackResponse.Create(
				[
					new BinPackResult()
					{
						Bin = new Bin { ID = "custom_bin_2", Length = 20, Width = 40, Height = 60 },
						Result = BinPackResultStatus.FullyPacked,
						PackedItems =
						[
							new PackedBox()
							{
								ID = "box_1",
								Length = 2,
								Width = 5,
								Height = 10,
								X = 0,
								Y = 0,
								Z = 0
							}
						],
						UnpackedItems =
						[
							new UnpackedBox()
							{
								ID = "box_2",
								Quantity = 1
							}
						],
						PackedItemsVolumePercentage = 2.70m,
						PackedBinVolumePercentage = 0.42m,
					}
				]
			));
	}
}
