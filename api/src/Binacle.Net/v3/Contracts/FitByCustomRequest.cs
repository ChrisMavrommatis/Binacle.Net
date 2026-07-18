using Binacle.Net.Kernel.OpenApi.Helpers;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OpenApiExamples;
using OpenApiExamples.Abstractions;
using System.ComponentModel;

namespace Binacle.Net.v3.Contracts;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

[Description("A request to fit items using custom bins.")]
public class FitByCustomRequest : IWithFittingParameters, IWithBins, IWithItems
{
	[Description(SchemaDescriptions.Parameters)]
	public required FitRequestParameters Parameters { get; set; } 
	
	[Description(SchemaDescriptions.Bins)]
	public required List<Bin> Bins { get; set; }
	
	[Description(SchemaDescriptions.Items)]
	public required List<Box> Items { get; set; } 
}

internal class FitByCustomRequestValidator : AbstractValidator<FitByCustomRequest>
{
	public FitByCustomRequestValidator()
	{
		Include(new FitRequestParametersValidator());
		Include(new BinsValidator());
		Include(new ItemsValidator());
	}
}


internal class FitByCustomRequestExample : ISingleOpenApiExamplesProvider<FitByCustomRequest>
{
	public IOpenApiExample<FitByCustomRequest> GetExample()
	{
		return OpenApiExample.Create(
			"customFitRequest",
			"Custom Fit Request",
			new FitByCustomRequest()
			{
				Parameters = new FitRequestParameters
				{
					Algorithm = Algorithm.FFD,
				},
				Bins = ExampleData.Bins("custom_bin"),
				Items = ExampleData.Items()
			});
	}
}

internal class FitByCustomResponseExamples : IMultipleOpenApiExamplesProvider<FitResponse>
{
	public IEnumerable<IOpenApiExample<FitResponse>> GetExamples()
	{
		var bins = ExampleData.Bins("custom_bin");

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
						ExampleData.SingleBin("custom_bin"),
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
						ExampleData.SingleBin("custom_bin"),
						BinFitResultStatus.EarlyFail_TotalVolumeExceeded,
						[],
						ExampleData.OversizedItemUnfitted()
					)
				]
			)
		);

	}
}

internal class FitByCustomValidationProblemExamples : IMultipleOpenApiExamplesProvider<ProblemDetails>
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
			"invalidBinData",
			"Invalid Bin Data",
			"Example response when you provide invalid Bin data",
			new Dictionary<string, string[]>()
			{
				{ "Bins", ["IDs in `Bins` must be unique"] },
				{ "Bins[0].Length", ["'Length' must be greater than '0'."] }
			}
		);
		
		yield return OpenApiValidationProblemExample.Create(
			"invalidItemData",
			"Invalid Item Data",
			"Example response when you provide invalid Item data",
			new Dictionary<string, string[]>()
			{
				{ "Items[1].Length", ["'Length' must be less than or equal to '65535'."] }
			}
		);
	}
}
