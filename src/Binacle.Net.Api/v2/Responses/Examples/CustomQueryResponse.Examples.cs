using ChrisMavrommatis.SwaggerExamples;
using ChrisMavrommatis.SwaggerExamples.Abstractions;

namespace Binacle.Net.Api.v2.Responses.Examples;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class CustomFitResponseExamples : MultipleSwaggerExamplesProvider<FitResponse>
{
	public override IEnumerable<ISwaggerExample<FitResponse>> GetExamples()
	{
		yield return SwaggerExample.Create("fullresponse", "Full Response", "Response example when you opt in to report all bins and all items.", FitResponse.Create(
			[
				new v2.Models.BinFitResult()
				{
					Bin = new v2.Models.Bin  { ID = "custom_bin_1", Length = 10, Width = 40, Height = 60 },
					Result = Models.BinFitResultStatus.AllItemsFit,
					FittedItems = [
						new v2.Models.ResultBox
						{
							ID = "box_1",
							Dimensions = new Models.Dimensions(2, 5, 10)
						},
						new v2.Models.ResultBox
						{
							ID = "box_2",
							Dimensions = new Models.Dimensions(12, 15, 10)
						},
					],
					UnfittedItems = [],
					FittedItemsVolumePercentage = 100.00m,
					FittedBinVolumePercentage = 8.33m,
				},
				new v2.Models.BinFitResult()
				{
					Bin = new v2.Models.Bin { ID = "custom_bin_2", Length = 20, Width = 40, Height = 60 },
					Result = Models.BinFitResultStatus.AllItemsFit,
					FittedItems = [
						new v2.Models.ResultBox
						{
							ID = "box_1",
							Dimensions = new Models.Dimensions(2, 5, 10)
						},
						new v2.Models.ResultBox
						{
							ID = "box_2",
							Dimensions = new Models.Dimensions(12, 15, 10)
						},
					],
					UnfittedItems = [],
					FittedItemsVolumePercentage = 100.00m,
					FittedBinVolumePercentage = 4.17m,
				}
			]
		));


		yield return SwaggerExample.Create("itemsomittedresponse", "Items Omitted Response", "Response example when you omit the items.", FitResponse.Create(
			[
				new v2.Models.BinFitResult()
				{
					Bin = new v2.Models.Bin  { ID = "custom_bin_1", Length = 10, Width = 40, Height = 60 },
					Result = Models.BinFitResultStatus.AllItemsFit,
					FittedItemsVolumePercentage = 100.00m,
					FittedBinVolumePercentage = 8.33m,
				},
				new v2.Models.BinFitResult()
				{
					Bin = new v2.Models.Bin { ID = "custom_bin_2", Length = 20, Width = 40, Height = 60 },
					Result = Models.BinFitResultStatus.AllItemsFit,
					FittedItemsVolumePercentage = 100.00m,
					FittedBinVolumePercentage = 4.17m,
				}
			]
		));


		yield return SwaggerExample.Create("smallestbinresponse", "Smallest Bin Response", "Response example when you request to report only the smallest bin.", FitResponse.Create(
			[
				new v2.Models.BinFitResult()
				{
					Bin = new v2.Models.Bin  { ID = "custom_bin_1", Length = 10, Width = 40, Height = 60 },
					Result = Models.BinFitResultStatus.AllItemsFit,
					FittedItemsVolumePercentage = 100.00m,
					FittedBinVolumePercentage = 8.33m,
				}
			]
		));



		yield return SwaggerExample.Create("binnotfitresponse", "Bin Not Fit Response", "Response example when a bin can't accommodate all the items", FitResponse.Create(
			[
				new v2.Models.BinFitResult()
				{
					Bin = new v2.Models.Bin { ID = "custom_small_bin_1", Length = 20, Width = 20, Height = 15 },
					FittedItems = [
					   new v2.Models.ResultBox
						{
							ID = "box_2",
							Dimensions = new Models.Dimensions(12, 15, 10)
						}
				   ],
					UnfittedItems = [
					   new v2.Models.ResultBox
						{
							ID = "box_1",
							Dimensions = new Models.Dimensions(2, 5, 10)
						},
						new v2.Models.ResultBox
						{
							ID = "box_2",
							Dimensions = new Models.Dimensions(12, 15, 10)
						},
					],
					Result = Models.BinFitResultStatus.NotAllItemsFit,
					FittedItemsVolumePercentage = 48.65m,
					FittedBinVolumePercentage = 30.00m,
				}
			]
		));


		yield return SwaggerExample.Create("earlyfailresponse", "Early fail Response", "Response example when a bin can't accommodate all the items due to an early fail check", FitResponse.Create(
			[
				new v2.Models.BinFitResult()
				{
					Bin = new v2.Models.Bin  { ID = "custom_small_bin_1", Length = 10, Width = 10, Height = 10 },
					FittedItems = [],
					UnfittedItems = [
						new v2.Models.ResultBox
						{
							ID = "box_2",
							Dimensions = new Models.Dimensions(12, 15, 10)
						},
						new v2.Models.ResultBox
						{
							ID = "box_1",
							Dimensions = new Models.Dimensions(2, 5, 10)
						},
						new v2.Models.ResultBox
						{
							ID = "box_2",
							Dimensions = new Models.Dimensions(12, 15, 10)
						},
					],
					Result = Models.BinFitResultStatus.EarlyFail_TotalVolumeExceeded,
					FittedItemsVolumePercentage = 0.00m,
					FittedBinVolumePercentage = 0.00m,
				}
			]
		));

	}
}



