using System.ComponentModel;
using Binacle.Net.Kernel.OpenApi.Helpers;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.v3.Contracts;

#pragma warning disable CS1591

[Description("A request to fit items using a preset.")]
public class FitByPresetRequest : IWithFittingParameters, IWithItems
{
	[Description(SchemaDescriptions.Parameters)]
	public required FitRequestParameters Parameters { get; set; }
	
	[Description(SchemaDescriptions.Items)]
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
				Items = ExampleData.Items()
			}
		);
	}
}

internal class FitByPresetResponseExamples : IMultipleOpenApiExamplesProvider<FitResponse>
{
	public IEnumerable<IOpenApiExample<FitResponse>> GetExamples()
	{
		var bins = ExampleData.Bins("preset_bin");

		yield return OpenApiExample.Create(
			"fullresponse",
			"Full Response",
			"Response Example indicating all items fit.",
			FitResponse.Create(
				[
					ExampleData.FittedResult(
						bins[0],
						BinFitResultStatus.AllItemsFit,
						ExampleData.AllItemsFitted(), 
						[]
					),
					ExampleData.FittedResult(
						bins[1], 
						BinFitResultStatus.AllItemsFit,
						ExampleData.AllItemsFitted(), 
						[]
					)
				]
			));

		yield return OpenApiExample.Create(
			"binnotfitresponse",
			"Bin Not Fit Response",
			"Response example when a bin can't accommodate all the items",
			FitResponse.Create(
				[
					ExampleData.FittedResult(
						ExampleData.SingleBin("preset_bin"),
						BinFitResultStatus.NotAllItemsFit,
						ExampleData.SomeItemsFitted(), 
						ExampleData.SomeItemsUnfitted()
					)
				]
			));


		yield return OpenApiExample.Create(
			"earlyfailresponse",
			"Early fail Response",
			"Response example when a bin can't accommodate all the items due to an early fail check",
			FitResponse.Create(
				[
					ExampleData.FittedResult(
						ExampleData.SingleBin("preset_bin"),
						BinFitResultStatus.EarlyFail_TotalVolumeExceeded,
						[],
						ExampleData.OversizedItemUnfitted()
					)
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
			"invalidItemData",
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
