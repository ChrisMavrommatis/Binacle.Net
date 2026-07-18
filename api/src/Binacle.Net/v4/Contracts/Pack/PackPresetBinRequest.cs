using OpenApiExamples;
using OpenApiExamples.Abstractions;
using System.ComponentModel;

namespace Binacle.Net.v4.Contracts.Pack;

#pragma warning disable CS1591

[Description("A request to pack items into a bin from a preset.")]
public class PackPresetBinRequest : PresetBinRequestBase;

internal class PackPresetBinRequestValidator : PresetBinRequestBaseValidator<PackPresetBinRequest>;

internal class PackPresetBinRequestExample : ISingleOpenApiExamplesProvider<PackPresetBinRequest>
{
	public IOpenApiExample<PackPresetBinRequest> GetExample()
	{
		return OpenApiExample.Create(
			"packPresetBinRequest",
			"Pack Preset Bin Request",
			new PackPresetBinRequest()
			{
				Parameters = new OperationParameters()
				{
					Algorithm = Algorithm.Best,
					IncludeViPaqData = true,
				},
				Items = ExampleData.Items()
			});
	}
}


internal class PackPresetBinResponseExamples : IMultipleOpenApiExamplesProvider<PackBinResponse>
{
	public IEnumerable<IOpenApiExample<PackBinResponse>> GetExamples()
	{
		yield return OpenApiExample.Create(
			"fullyPackedResponse",
			"Fully Packed Response",
			"Example response when all items fit into the bin and no items are left unpacked.",
			PackExampleResponses.FullyPacked("preset_bin_1"));

		yield return OpenApiExample.Create(
			"partiallyPackedResponse",
			"Partially Packed Response",
			"Example response when some items fit into the bin but some items are left unpacked",
			PackExampleResponses.PartiallyPacked("preset_bin_1"));

		yield return OpenApiExample.Create(
			"unpackedResponse",
			"Unpacked Response",
			"Example response when no items fit into the bin and all items are left unpacked",
			PackExampleResponses.NotPacked("preset_bin_1"));
	}
}
