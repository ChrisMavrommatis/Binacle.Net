using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.v4.Contracts.Fit;

#pragma warning disable CS1591

public class FitPresetCompareRequest : PresetBinsRequestBase;

internal class FitPresetCompareRequestValidator : PresetBinsRequestBaseValidator<FitPresetCompareRequest>;

internal class FitPresetCompareRequestExample : ISingleOpenApiExamplesProvider<FitPresetCompareRequest>
{
	public IOpenApiExample<FitPresetCompareRequest> GetExample()
	{
		return OpenApiExample.Create(
			"fitPresetCompareRequest",
			"Fit Preset Compare Request",
			new FitPresetCompareRequest()
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

internal class FitPresetCompareResponseExamples : IMultipleOpenApiExamplesProvider<FitCompareResponse>
{
	public IEnumerable<IOpenApiExample<FitCompareResponse>> GetExamples()
	{
		// The items fit every bin in the preset, which is the point of comparing: the outcome is the same and
		// only the utilization separates them, so the caller can see what each bin would cost them.
		yield return OpenApiExample.Create(
			"compareResponse",
			"Compare Response",
			"Example response with one result per bin in the preset, in the order the preset defines them.",
			new FitCompareResponse
			{
				Results = ExampleData.Bins("preset_bin")
					.Select(bin => FitExampleResponses.Fits(bin))
					.ToList()
			}
		);
	}
}
