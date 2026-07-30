using System.ComponentModel;
using Binacle.Net.Kernel.OpenApi.Helpers;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.v3.Contracts;

#pragma warning disable CS1591
[Description("A request to pack items using a preset.")]
public class PackByPresetRequest : IWithPackingParameters, IWithItems
{
	[Description(SchemaDescriptions.Parameters)]
	public required PackRequestParameters Parameters { get; set; } 
	
	[Description(SchemaDescriptions.Items)]
	public required List<Box> Items { get; set; } 
}

internal class PackByPresetRequestValidator : AbstractValidator<PackByPresetRequest>
{
	public PackByPresetRequestValidator()
	{
		Include(new PackRequestParametersValidator());
		Include(new ItemsValidator());
	}
}

internal class PackByPresetRequestExample : ISingleOpenApiExamplesProvider<PackByPresetRequest>
{
	public IOpenApiExample<PackByPresetRequest> GetExample()
	{
		return OpenApiExample.Create(
			"presetPackRequest",
			"Preset Pack Request",
			new PackByPresetRequest
			{
				Parameters = new PackRequestParameters
				{
					Algorithm = Algorithm.FFD,
					IncludeViPaqData = true,
				},
				Items = ExampleData.Items()
			}
		);
	}
}

internal class PackByPresetResponseExamples : IMultipleOpenApiExamplesProvider<PackResponse>
{
	public IEnumerable<IOpenApiExample<PackResponse>> GetExamples()
	{
		var bins = ExampleData.Bins("preset_bin");

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
			)
		);

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
			)
		);
	}
}


internal class PackByPresetValidationProblemExamples : IMultipleOpenApiExamplesProvider<ProblemDetails>
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
