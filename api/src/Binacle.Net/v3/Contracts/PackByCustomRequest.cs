using Binacle.Net.Kernel.OpenApi.Helpers;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OpenApiExamples;
using OpenApiExamples.Abstractions;
using System.ComponentModel;

namespace Binacle.Net.v3.Contracts;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

[Description("A request to pack items using custom bins.")]
public class PackByCustomRequest : IWithPackingParameters, IWithBins, IWithItems
{
	[Description(SchemaDescriptions.Parameters)]
	public required PackRequestParameters Parameters { get; set; } 
	
	[Description(SchemaDescriptions.Bins)]
	public required List<Bin> Bins { get; set; } 
	
	[Description(SchemaDescriptions.Items)]
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
				Bins = ExampleData.Bins("custom_bin"),
				Items = ExampleData.Items()
			});
	}
}

internal class PackByCustomResponseExamples : IMultipleOpenApiExamplesProvider<PackResponse>
{
	public IEnumerable<IOpenApiExample<PackResponse>> GetExamples()
	{
		var bins = ExampleData.Bins("custom_bin");

		yield return OpenApiExample.Create(
			"fullypackedresponse",
			"Fully Packed Response",
			"Fully Packed Response example.",
			PackResponse.Create(
				[
					ExampleData.PackedResult(
						bins[0], 
						BinPackResultStatus.FullyPacked,
						ExampleData.AllItemsPacked(), 
						[]
					).WithViPaqData(),
					ExampleData.PackedResult(
						bins[1], 
						BinPackResultStatus.FullyPacked,
						ExampleData.AllItemsPacked(), 
						[]
					).WithViPaqData()
				]
			));

		yield return OpenApiExample.Create(
			"partiallypackedresponse",
			"Partially Packed Response",
			"Partially Packed Response example.",
			PackResponse.Create(
				[
					ExampleData.PackedResult(
						bins[1], 
						BinPackResultStatus.PartiallyPacked,
						ExampleData.SomeItemsPacked(), 
						ExampleData.SomeItemsUnpacked()
					).WithViPaqData()
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
