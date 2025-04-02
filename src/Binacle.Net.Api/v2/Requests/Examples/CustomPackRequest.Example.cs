// using ChrisMavrommatis.SwaggerExamples;
//
// namespace Binacle.Net.Api.v2.Requests.Examples;
//
// internal class CustomPackRequestExample : SingleSwaggerExamplesProvider<CustomPackRequest>
// {
// 	public override CustomPackRequest GetExample()
// 	{
// 		return new CustomPackRequest
// 		{
// 			Parameters = new PackRequestParameters
// 			{
// 				NeverReportUnpackedItems = false,
// 				OptInToEarlyFails = false,
// 				StopAtSmallestBin = false,
// 				ReportPackedItemsOnlyWhenFullyPacked = false
// 			},
// 			Bins = new List<v2.Models.Bin>
// 			{
// 				new () { ID = "custom_bin_1", Length = 10, Width = 40, Height = 60 },
// 				new () { ID = "custom_bin_2", Length = 20, Width = 40, Height = 60 },
//
// 			},
// 			Items = new List<v2.Models.Box>
// 			{
// 				new () { ID = "box_1", Quantity = 2, Length = 2, Width = 5, Height = 10 },
// 				new () { ID = "box_2", Quantity = 1, Length = 12, Width = 15, Height = 10 },
// 				new() { ID = "box_3", Quantity = 1, Length = 12, Width = 10, Height = 15 },
// 			}
// 		};
// 	}
// }
