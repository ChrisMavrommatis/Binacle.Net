using Binacle.Net.Kernel.OpenApi.Helpers;
using Binacle.Net.Models;
using Binacle.Net.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.v3.Contracts;

#pragma warning disable CS1591
public class FitByPresetRequest : IWithFittingParameters, IWithItems
{
	public required FitRequestParameters Parameters { get; set; }
	public required List<Box> Items { get; set; }
}

internal class FitByPresetRequestValidator : AbstractValidator<FitByPresetRequest>
{
	public FitByPresetRequestValidator()
	{
		Include(new FitRequestParametersValidator());
		Include(new ItemsValidator());
	}
}

internal class FitByPresetRequestExample : ISingleOpenApiExamplesProvider<FitByPresetRequest>
{
	public IOpenApiExample<FitByPresetRequest> GetExample()
	{
		return OpenApiExample.Create(
			"presetFitRequest",
			"Preset Fit  Request",
			new FitByPresetRequest
			{
				Parameters = new FitRequestParameters
				{
					Algorithm = Algorithm.FFD,
				},
				Items = new List<Box>
				{
					new() { ID = "box_1", Quantity = 2, Length = 2, Width = 5, Height = 10 },
					new() { ID = "box_2", Quantity = 1, Length = 12, Width = 15, Height = 10 },
					new() { ID = "box_3", Quantity = 1, Length = 12, Width = 10, Height = 15 },
				}
			}
		);
	}
}

internal class FitByPresetResponseExamples : IMultipleOpenApiExamplesProvider<FitResponse>
{
	public IEnumerable<IOpenApiExample<FitResponse>> GetExamples()
	{
		yield return OpenApiExample.Create(
			"fullresponse",
			"Full Response",
			"Response Example indicating all items fit.",
			FitResponse.Create(
				[
					new BinFitResult()
					{
						Bin = new Bin { ID = "preset_bin_1", Length = 10, Width = 40, Height = 60 },
						Result = BinFitResultStatus.AllItemsFit,
						FittedItems =
						[
							new FittedBox()
							{
								ID = "box_2",
								Length = 10,
								Width = 12,
								Height = 15,
							},
							new FittedBox
							{
								ID = "box_1",
								Length = 2,
								Width = 5,
								Height = 10,
							},
						],
						UnfittedItems = [],
						FittedItemsVolumePercentage = 100.00m,
						FittedBinVolumePercentage = 8.33m,
					},
					new BinFitResult()
					{
						Bin = new Bin { ID = "preset_bin_2", Length = 20, Width = 40, Height = 60 },
						Result = BinFitResultStatus.AllItemsFit,
						FittedItems =
						[
							new FittedBox()
							{
								ID = "box_2",
								Length = 12,
								Width = 15,
								Height = 10,
							},
							new FittedBox()
							{
								ID = "box_1",
								Length = 2,
								Width = 5,
								Height = 10,
							}
						],
						UnfittedItems = [],
						FittedItemsVolumePercentage = 100.00m,
						FittedBinVolumePercentage = 3.96m,
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
						Bin = new Bin { ID = "preset_small_bin_1", Length = 20, Width = 20, Height = 15 },
						FittedItems =
						[
							new FittedBox
							{
								ID = "box_2",
								Length = 12,
								Width = 15,
								Height = 10,
							}
						],
						UnfittedItems =
						[
							new UnfittedBox
							{
								ID = "box_1",
								Quantity = 1
							},
							new UnfittedBox
							{
								ID = "box_2",
								Quantity = 1
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
						Bin = new Bin { ID = "preset_small_bin_1", Length = 10, Width = 10, Height = 10 },
						FittedItems = [],
						UnfittedItems =
						[
							new UnfittedBox
							{
								ID = "box_2",
								Quantity = 2
							},
							new UnfittedBox
							{
								ID = "box_1",
								Quantity = 1
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

internal class FitByPresetValidationProblemExamples : IMultipleOpenApiExamplesProvider<ProblemDetails>
{
	public IEnumerable<IOpenApiExample<ProblemDetails>> GetExamples()
	{
		yield return OpenApiValidationProblemExample.Create(
			"invalidAlgorithm",
			"Invalid Algorithm",
			"Example response when you provide invalid algorithm",
			new Dictionary<string, string[]>()
			{
				{
					"Parameters.Algorithm",
					[ErrorMessage.RequiredEnumValues<Algorithm>(nameof(IWithAlgorithm.Algorithm))]
				}
			}
		);

		yield return OpenApiValidationProblemExample.Create(
			"ivalidItemData",
			"Invalid Item Data",
			"Example response when you provide invalid Item data",
			new Dictionary<string, string[]>()
			{
				{ "Items", ["IDs in `Items` must be unique"] },
				{ "Items[1].Length", ["'Length' must be less than or equal to '65535'."] }
			}
		);
	}
}
