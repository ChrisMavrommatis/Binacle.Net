using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.v4.Contracts.Fit;

#pragma warning disable CS1591

public class FitPresetSmallestBinRequest : PresetBinsRequestBase;

internal class FitPresetSmallestBinRequestValidator : PresetBinsRequestBaseValidator<FitPresetSmallestBinRequest>;

internal class FitPresetSmallestBinRequestExample : ISingleOpenApiExamplesProvider<FitPresetSmallestBinRequest>
{
	public IOpenApiExample<FitPresetSmallestBinRequest> GetExample()
	{
		return OpenApiExample.Create(
			"fitPresetSmallestBinRequest",
			"Fit Preset Smallest Bin Request",
			new FitPresetSmallestBinRequest()
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

internal class FitPresetSmallestBinResponseExamples : IMultipleOpenApiExamplesProvider<FitBinResponse>
{
	public IEnumerable<IOpenApiExample<FitBinResponse>> GetExamples()
	{
		yield return OpenApiExample.Create(
			"fitResponse",
			"Fit Response",
			"Example response for the smallest bin in the preset that the items fit into.",
			FitExampleResponses.Fits("preset_bin_1"));

		yield return OpenApiExample.Create(
			"doesNotFitResponse",
			"Does Not Fit Response",
			"Example response when the items don't fit into any bin in the preset.",
			FitExampleResponses.DoesNotFit("preset_bin_1"));
	}
}
