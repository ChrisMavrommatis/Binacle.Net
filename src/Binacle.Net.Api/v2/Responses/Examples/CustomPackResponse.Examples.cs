// using ChrisMavrommatis.SwaggerExamples;
// using ChrisMavrommatis.SwaggerExamples.Abstractions;
//
// namespace Binacle.Net.Api.v2.Responses.Examples;
//
// internal class CustomPackResponseExamples : MultipleSwaggerExamplesProvider<PackResponse>
// {
// 	public override IEnumerable<ISwaggerExample<PackResponse>> GetExamples()
// 	{
// 		yield return SwaggerExample.Create("fullypackedresponse", "Fully Packed Response", "Fully Packed Response example.", PackResponse.Create(
// 			[
// 				new v2.Models.BinPackResult()
// 				{
// 					Bin = new v2.Models.Bin  { ID = "custom_bin_1", Length = 10, Width = 40, Height = 60 },
// 					Result = Models.BinPackResultStatus.FullyPacked,
// 					PackedItems = [
// 						new v2.Models.ResultBox
// 						{
// 							ID = "box_2",
// 							Dimensions = new Models.Dimensions(10, 12, 15),
// 							Coordinates = new Models.Coordinates(0, 0, 0)
// 						},
// 						new v2.Models.ResultBox
// 						{
// 							ID = "box_1",
// 							Dimensions = new Models.Dimensions(2, 5, 10),
// 							Coordinates = new Models.Coordinates(0, 12, 0)
// 						},
//
// 					],
// 					UnpackedItems = [],
// 					PackedItemsVolumePercentage = 100.00m,
// 					PackedBinVolumePercentage = 7.92m,
// 				},
// 				new v2.Models.BinPackResult()
// 				{
// 					Bin = new v2.Models.Bin { ID = "custom_bin_2", Length = 20, Width = 40, Height = 60 },
// 					Result = Models.BinPackResultStatus.FullyPacked,
// 					PackedItems = [
// 						new v2.Models.ResultBox
// 						{
// 							ID = "box_2",
// 							Dimensions = new Models.Dimensions(12, 15, 10),
// 							Coordinates = new Models.Coordinates(0, 0, 0)
// 						},
// 						new v2.Models.ResultBox
// 						{
// 							ID = "box_1",
// 							Dimensions = new Models.Dimensions(2, 5, 10),
// 							Coordinates = new Models.Coordinates(12, 0, 0)
// 						},
//
// 					],
// 					UnpackedItems = [],
// 					PackedItemsVolumePercentage = 100.00m,
// 					PackedBinVolumePercentage = 3.96m,
// 				}
// 			]
// 		));
//
//
// 		yield return SwaggerExample.Create("partiallypackedresponse", "Partially Packed Response", "Partially Packed Response example.", PackResponse.Create(
// 			[
// 				new v2.Models.BinPackResult()
// 				{
// 					Bin = new v2.Models.Bin { ID = "custom_bin_2", Length = 20, Width = 40, Height = 60 },
// 					Result = Models.BinPackResultStatus.FullyPacked,
// 					PackedItems = [
// 						new v2.Models.ResultBox
// 						{
// 							ID = "box_1",
// 							Dimensions = new Models.Dimensions(2, 5, 10),
// 							Coordinates = new Models.Coordinates(0, 0, 0)
// 						}
// 					],
// 					UnpackedItems = [
// 						new v2.Models.ResultBox
// 						{
// 							ID = "box_2",
// 							Dimensions = new Models.Dimensions(12, 15, 20)
// 						}
// 					],
// 					PackedItemsVolumePercentage = 2.70m,
// 					PackedBinVolumePercentage = 0.42m,
// 				}
// 			]
// 		));
// 	}
// }
