using OpenApiExamples;
using OpenApiExamples.Abstractions;
using System.ComponentModel;

namespace Binacle.Net.v4.Contracts.Pack;

#pragma warning disable CS1591

[Description("A request to pack items into the best bin of a preset.")]
public class PackPresetBestBinRequest : PresetBinsRequestBase;

internal class PackPresetBestBinRequestValidator : PresetBinsRequestBaseValidator<PackPresetBestBinRequest>;

internal class PackPresetBestBinRequestExample : ISingleOpenApiExamplesProvider<PackPresetBestBinRequest>
{
	public IOpenApiExample<PackPresetBestBinRequest> GetExample()
	{
		return OpenApiExample.Create(
			"packPresetBestBinRequest",
			"Pack Preset Best Bin Request",
			new PackPresetBestBinRequest()
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

internal class PackPresetBestBinResponseExamples : IMultipleOpenApiExamplesProvider<PackBinResponse>
{
	public IEnumerable<IOpenApiExample<PackBinResponse>> GetExamples()
	{
		yield return OpenApiExample.Create(
			"fullyPackedResponse",
			"Fully Packed Response",
			"Example response for the bin in the preset whose volume the items fill the most.",
			PackExampleResponses.FullyPacked("preset_bin_1"));

		yield return OpenApiExample.Create(
			"partiallyPackedResponse",
			"Partially Packed Response",
			"Example response when no bin in the preset fits every item, so the one the items fill the most is returned.",
			PackExampleResponses.PartiallyPacked("preset_bin_1"));

		yield return OpenApiExample.Create(
			"unpackedResponse",
			"Unpacked Response",
			"Example response when no bin in the preset can fit the items.",
			PackExampleResponses.NotPacked("preset_bin_1"));
	}
}
