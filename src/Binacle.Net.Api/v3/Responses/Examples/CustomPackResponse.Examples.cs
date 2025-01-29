using ChrisMavrommatis.SwaggerExamples;
using ChrisMavrommatis.SwaggerExamples.Abstractions;

namespace Binacle.Net.Api.v3.Responses.Examples;

internal class CustomPackResponseExamples : MultipleSwaggerExamplesProvider<PackResponse>
{
	public override IEnumerable<ISwaggerExample<PackResponse>> GetExamples()
	{
		yield return SwaggerExample.Create("fullypackedresponse", "Fully Packed Response", "Fully Packed Response example.", PackResponse.Create(
			[
				new v3.Models.BinPackResult()
				{
					Bin = new v3.Models.Bin  { ID = "custom_bin_1", Length = 10, Width = 40, Height = 60 },
					Result = Models.BinPackResultStatus.FullyPacked,
					PackedItems = [
						new v3.Models.PackedBox()
						{
							ID = "box_2",
							Length = 10,
							Width = 12,
							Height = 15,
							X = 0,
							Y = 0,
							Z = 0
						},
						new v3.Models.PackedBox
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
				new v3.Models.BinPackResult()
				{
					Bin = new v3.Models.Bin { ID = "custom_bin_2", Length = 20, Width = 40, Height = 60 },
					Result = Models.BinPackResultStatus.FullyPacked,
					PackedItems = [
						new v3.Models.PackedBox()
						{
							ID = "box_2",
							Length = 12,
							Width = 15,
							Height = 10,
							X = 0,
							Y = 0,
							Z = 0
						},
						new v3.Models.PackedBox()
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


		yield return SwaggerExample.Create("partiallypackedresponse", "Partially Packed Response", "Partially Packed Response example.", PackResponse.Create(
			[
				new v3.Models.BinPackResult()
				{
					Bin = new v3.Models.Bin { ID = "custom_bin_2", Length = 20, Width = 40, Height = 60 },
					Result = Models.BinPackResultStatus.FullyPacked,
					PackedItems = [
						new v3.Models.PackedBox()
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
					UnpackedItems = [
						new v3.Models.UnpackedBox()
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
