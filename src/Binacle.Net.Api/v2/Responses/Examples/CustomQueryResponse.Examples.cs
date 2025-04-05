using Binacle.Net.Api.v2.Models;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.Api.v2.Responses.Examples;

internal class CustomFitResponseExamples : IMultipleOpenApiExamplesProvider<FitResponse>
{
	public IEnumerable<IOpenApiExample<FitResponse>> GetExamples()
	{
		yield return OpenApiExample.Create(
			"fullresponse",
			"Full Response",
			"Response example when you opt in to report all bins and all items.",
			FitResponse.Create(
				[
					new BinFitResult()
					{
						Bin = new Bin { ID = "custom_bin_1", Length = 10, Width = 40, Height = 60 },
						Result = BinFitResultStatus.AllItemsFit,
						FittedItems =
						[
							new ResultBox
							{
								ID = "box_1",
								Dimensions = new Dimensions(2, 5, 10)
							},
							new ResultBox
							{
								ID = "box_2",
								Dimensions = new Dimensions(12, 15, 10)
							},
						],
						UnfittedItems = [],
						FittedItemsVolumePercentage = 100.00m,
						FittedBinVolumePercentage = 8.33m,
					},
					new BinFitResult()
					{
						Bin = new Bin { ID = "custom_bin_2", Length = 20, Width = 40, Height = 60 },
						Result = BinFitResultStatus.AllItemsFit,
						FittedItems =
						[
							new ResultBox
							{
								ID = "box_1",
								Dimensions = new Dimensions(2, 5, 10)
							},
							new ResultBox
							{
								ID = "box_2",
								Dimensions = new Dimensions(12, 15, 10)
							},
						],
						UnfittedItems = [],
						FittedItemsVolumePercentage = 100.00m,
						FittedBinVolumePercentage = 4.17m,
					}
				]
			));


		yield return OpenApiExample.Create(
			"itemsomittedresponse",
			"Items Omitted Response",
			"Response example when you omit the items.",
			FitResponse.Create(
				[
					new BinFitResult()
					{
						Bin = new Bin { ID = "custom_bin_1", Length = 10, Width = 40, Height = 60 },
						Result = BinFitResultStatus.AllItemsFit,
						FittedItemsVolumePercentage = 100.00m,
						FittedBinVolumePercentage = 8.33m,
					},
					new BinFitResult()
					{
						Bin = new Bin { ID = "custom_bin_2", Length = 20, Width = 40, Height = 60 },
						Result = BinFitResultStatus.AllItemsFit,
						FittedItemsVolumePercentage = 100.00m,
						FittedBinVolumePercentage = 4.17m,
					}
				]
			)
		);


		yield return OpenApiExample.Create(
			"smallestbinresponse",
			"Smallest Bin Response",
			"Response example when you request to report only the smallest bin.",
			FitResponse.Create(
				[
					new BinFitResult()
					{
						Bin = new Bin { ID = "custom_bin_1", Length = 10, Width = 40, Height = 60 },
						Result = BinFitResultStatus.AllItemsFit,
						FittedItemsVolumePercentage = 100.00m,
						FittedBinVolumePercentage = 8.33m,
					}
				]
			));


		yield return OpenApiExample.Create(
			"binnotfitresponse",
			"Bin Not Fit Response",
			"Response example when a bin can't accommodate all the items",
			FitResponse.Create(
				[
					new BinFitResult()
					{
						Bin = new Bin { ID = "custom_small_bin_1", Length = 20, Width = 20, Height = 15 },
						FittedItems =
						[
							new ResultBox
							{
								ID = "box_2",
								Dimensions = new Dimensions(12, 15, 10)
							}
						],
						UnfittedItems =
						[
							new ResultBox
							{
								ID = "box_1",
								Dimensions = new Dimensions(2, 5, 10)
							},
							new ResultBox
							{
								ID = "box_2",
								Dimensions = new Dimensions(12, 15, 10)
							},
						],
						Result = BinFitResultStatus.NotAllItemsFit,
						FittedItemsVolumePercentage = 48.65m,
						FittedBinVolumePercentage = 30.00m,
					}
				]
			));


		yield return OpenApiExample.Create(
			"earlyfailresponse",
			"Early fail Response",
			"Response example when a bin can't accommodate all the items due to an early fail check",
			FitResponse.Create(
				[
					new BinFitResult()
					{
						Bin = new Bin { ID = "custom_small_bin_1", Length = 10, Width = 10, Height = 10 },
						FittedItems = [],
						UnfittedItems =
						[
							new ResultBox
							{
								ID = "box_2",
								Dimensions = new Dimensions(12, 15, 10)
							},
							new ResultBox
							{
								ID = "box_1",
								Dimensions = new Dimensions(2, 5, 10)
							},
							new ResultBox
							{
								ID = "box_2",
								Dimensions = new Dimensions(12, 15, 10)
							},
						],
						Result = BinFitResultStatus.EarlyFail_TotalVolumeExceeded,
						FittedItemsVolumePercentage = 0.00m,
						FittedBinVolumePercentage = 0.00m,
					}
				]
			)
		);
	}
}
