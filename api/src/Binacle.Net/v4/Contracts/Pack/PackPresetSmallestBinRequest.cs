using OpenApiExamples;
using OpenApiExamples.Abstractions;
using System.ComponentModel;

namespace Binacle.Net.v4.Contracts.Pack;

#pragma warning disable CS1591

[Description("A request to pack items into the smallest fitting bin of a preset.")]
public class PackPresetSmallestBinRequest : PresetBinsRequestBase;

internal class PackPresetSmallestBinRequestValidator : PresetBinsRequestBaseValidator<PackPresetSmallestBinRequest>;

internal class PackPresetSmallestBinRequestExample : ISingleOpenApiExamplesProvider<PackPresetSmallestBinRequest>
{
	public IOpenApiExample<PackPresetSmallestBinRequest> GetExample()
	{
		return OpenApiExample.Create(
			"packPresetSmallestBinRequest",
			"Pack Preset Smallest Bin Request",
			new PackPresetSmallestBinRequest()
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

internal class PackPresetSmallestBinResponseExamples : IMultipleOpenApiExamplesProvider<PackBinResponse>
{
	public IEnumerable<IOpenApiExample<PackBinResponse>> GetExamples()
	{
		yield return OpenApiExample.Create(
			"fullyPackedResponse",
			"Fully Packed Response",
			"Example response when all items fit into the smallest bin of the preset.",
			PackExampleResponses.FullyPacked("preset_bin_1"));

		yield return OpenApiExample.Create(
			"partiallyPackedResponse",
			"Partially Packed Response",
			"Example response when some items fit into the bin but some items are left unpacked",
			PackExampleResponses.PartiallyPacked("preset_bin_1"));

		yield return OpenApiExample.Create(
			"unpackedResponse",
			"Unpacked Response",
			"Example response when no bin in the preset can fit the items.",
			PackExampleResponses.NotPacked("preset_bin_1"));
	}
}
