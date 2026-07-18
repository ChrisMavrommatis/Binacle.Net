using FluentValidation;
using OpenApiExamples;
using OpenApiExamples.Abstractions;
using System.ComponentModel;

namespace Binacle.Net.v4.Contracts.Pack;

#pragma warning disable CS1591

[Description("A request to pack items into the smallest fitting custom bin.")]
public class PackCustomSmallestBinRequest : CustomBinsRequestBase;

internal class PackCustomSmallestBinRequestValidator : AbstractValidator<PackCustomSmallestBinRequest>
{
	public PackCustomSmallestBinRequestValidator()
	{
		Include(new CustomBinsRequestBaseValidator());
	}
}

internal class PackCustomSmallestBinRequestExample : ISingleOpenApiExamplesProvider<PackCustomSmallestBinRequest>
{
	public IOpenApiExample<PackCustomSmallestBinRequest> GetExample()
	{
		return OpenApiExample.Create(
			"packCustomSmallestBinRequest",
			"Pack Custom Smallest Bin Request",
			new PackCustomSmallestBinRequest()
			{
				Parameters = new OperationParameters()
				{
					Algorithm = Algorithm.Best,
					IncludeViPaqData = true,
				},
				Bins = ExampleData.Bins("custom_bin"),
				Items = ExampleData.Items()
			});
	}
}


internal class PackCustomSmallestBinResponseExamples : IMultipleOpenApiExamplesProvider<PackBinResponse>
{
	public IEnumerable<IOpenApiExample<PackBinResponse>> GetExamples()
	{
		yield return OpenApiExample.Create(
			"fullyPackedResponse",
			"Fully Packed Response",
			"Example response when all items fit into the bin and no items are left unpacked.",
			PackExampleResponses.FullyPacked("custom_bin_1"));

		yield return OpenApiExample.Create(
			"partiallyPackedResponse",
			"Partially Packed Response",
			"Example response when some items fit into the bin but some items are left unpacked",
			PackExampleResponses.PartiallyPacked("custom_bin_1"));

		yield return OpenApiExample.Create(
			"unpackedResponse",
			"Unpacked Response",
			"Example response when no items fit into the bin and all items are left unpacked",
			PackExampleResponses.NotPacked("custom_bin_1"));
	}
}
