using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.v4.Contracts.Pack;

#pragma warning disable CS1591

public class PackPresetCompareRequest : PresetBinsRequestBase;

internal class PackPresetCompareRequestValidator : PresetBinsRequestBaseValidator<PackPresetCompareRequest>;

internal class PackPresetCompareRequestExample : ISingleOpenApiExamplesProvider<PackPresetCompareRequest>
{
	public IOpenApiExample<PackPresetCompareRequest> GetExample()
	{
		return OpenApiExample.Create(
			"packPresetCompareRequest",
			"Pack Preset Compare Request",
			new PackPresetCompareRequest()
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

internal class PackPresetCompareResponseExamples : IMultipleOpenApiExamplesProvider<PackCompareResponse>
{
	public IEnumerable<IOpenApiExample<PackCompareResponse>> GetExamples()
	{
		yield return OpenApiExample.Create(
			"compareResponse",
			"Compare Response",
			"Example response with one result per bin in the preset, in the order the preset defines them.",
			new PackCompareResponse
			{
				Results = ExampleData.Bins("preset_bin")
					.Select(bin => PackExampleResponses.FullyPacked(bin))
					.ToList()
			}
		);
	}
}
