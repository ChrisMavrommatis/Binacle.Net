using Binacle.Net.Kernel.OpenApi.Helpers;
using Binacle.Net.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.v3.Contracts;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class PackByCustomRequest : IWithPackingParameters, IWithBins, IWithItems
{
	public required PackRequestParameters Parameters { get; set; } 
	public required List<Bin> Bins { get; set; } 
	public required List<Box> Items { get; set; } 
}

internal class PackByCustomRequestValidator : AbstractValidator<PackByCustomRequest>
{
	public PackByCustomRequestValidator()
	{
		Include(new PackRequestParametersValidator());
		Include(new BinsValidator());
		Include(new ItemsValidator());
	}
}


internal class PackByCustomRequestExample : ISingleOpenApiExamplesProvider<PackByCustomRequest>
{
	public IOpenApiExample<PackByCustomRequest> GetExample()
	{
		return OpenApiExample.Create(
			"customPackRequest",
			"Custom Pack Request",
			new PackByCustomRequest()
			{
				Parameters = new PackRequestParameters
				{
					Algorithm = Algorithm.FFD,
					IncludeViPaqData = true,
				},
				Bins = new List<Bin>
				{
					new() { ID = "custom_bin_1", Length = 10, Width = 40, Height = 60 },
					new() { ID = "custom_bin_2", Length = 20, Width = 40, Height = 60 },
				},
				Items = new List<Box>
				{
					new() { ID = "box_1", Quantity = 2, Length = 2, Width = 5, Height = 10 },
					new() { ID = "box_2", Quantity = 1, Length = 12, Width = 15, Height = 10 },
					new() { ID = "box_3", Quantity = 1, Length = 12, Width = 10, Height = 15 },
				}
			});
	}
}

internal class PackByCustomResponseExamples : IMultipleOpenApiExamplesProvider<PackResponse>
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
						ViPaqData = "AAQACig8CgwPAAAACgwPAAwAAgUKAAAPAgUKABgA"
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
						ViPaqData = "AAQAFCg8DA8KAAAADAoPAA8AAgUKDAAAAgUKAAAK"
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
						Result = BinPackResultStatus.PartiallyPacked,
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

internal class PackByCustomValidationProblemExamples : IMultipleOpenApiExamplesProvider<ProblemDetails>
{
	public IEnumerable<IOpenApiExample<ProblemDetails>> GetExamples()
	{
		yield return OpenApiValidationProblemExample.Create(
			"invalidAlgorithm",
			"Invalid Algorithm",
			"Example response when you provide invalid algorithm",
			new Dictionary<string, string[]>()
			{
				{ "Parameters.Algorithm", [ErrorMessage.RequiredEnumValues<Algorithm>(nameof(IWithAlgorithm.Algorithm))] }
			}
		);

		yield return OpenApiValidationProblemExample.Create(
			"ivalidBinData",
			"Invalid Bin Data",
			"Example response when you provide invalid Bin data",
			new Dictionary<string, string[]>()
			{
				{ "Bins", ["IDs in `Bins` must be unique"] },
				{ "Bins[0].Length", ["'Length' must be greater than '0'."] }
			}
		);
		
		yield return OpenApiValidationProblemExample.Create(
			"ivalidItemData",
			"Invalid Item Data",
			"Example response when you provide invalid Item data",
			new Dictionary<string, string[]>()
			{
				{ "Items[1].Length", ["'Length' must be less than or equal to '65535'."] }
			}
		);
	}
}
