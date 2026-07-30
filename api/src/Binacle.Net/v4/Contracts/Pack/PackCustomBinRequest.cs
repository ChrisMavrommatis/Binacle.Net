using OpenApiExamples;
using OpenApiExamples.Abstractions;
using System.ComponentModel;

namespace Binacle.Net.v4.Contracts.Pack;

#pragma warning disable CS1591

[Description("A request to pack items into a custom bin.")]
public class PackCustomBinRequest : CustomBinRequestBase;

internal class PackCustomBinRequestValidator : CustomBinRequestBaseValidator<PackCustomBinRequest>;

internal class PackCustomBinRequestExample : ISingleOpenApiExamplesProvider<PackCustomBinRequest>
{
	public IOpenApiExample<PackCustomBinRequest> GetExample()
	{
		return OpenApiExample.Create(
			"packCustomBinRequest",
			"Pack Custom Bin Request",
			new PackCustomBinRequest()
			{
				Parameters = new OperationParameters()
				{
					Algorithm = Algorithm.Best,
					IncludeViPaqData = true,
				},
				Bin = ExampleData.SingleBin("custom_bin"),
				Items = ExampleData.Items()
			});
	}
}


internal class PackCustomBinResponseExamples : IMultipleOpenApiExamplesProvider<PackBinResponse>
{
	public IEnumerable<IOpenApiExample<PackBinResponse>> GetExamples()
	{
		yield return OpenApiExample.Create(
			"fullyPackedResponse",
			"Fully Packed Response",
			"Example response when all items fit into the bin and no items are left unpacked.",
			PackExampleResponses.FullyPacked("custom_bin"));

		yield return OpenApiExample.Create(
			"partiallyPackedResponse",
			"Partially Packed Response",
			"Example response when some items fit into the bin but some items are left unpacked",
			PackExampleResponses.PartiallyPacked("custom_bin"));

		yield return OpenApiExample.Create(
			"unpackedResponse",
			"Unpacked Response",
			"Example response when no items fit into the bin and all items are left unpacked",
			PackExampleResponses.NotPacked("custom_bin"));
	}
}
