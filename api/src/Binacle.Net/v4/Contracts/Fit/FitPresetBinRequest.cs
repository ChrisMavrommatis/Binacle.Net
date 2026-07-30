using OpenApiExamples;
using OpenApiExamples.Abstractions;
using System.ComponentModel;

namespace Binacle.Net.v4.Contracts.Fit;

#pragma warning disable CS1591

[Description("A request to check whether items fit into a bin from a preset.")]
public class FitPresetBinRequest : PresetBinRequestBase;

internal class FitPresetBinRequestValidator : PresetBinRequestBaseValidator<FitPresetBinRequest>;

internal class FitPresetBinRequestExample : ISingleOpenApiExamplesProvider<FitPresetBinRequest>
{
	public IOpenApiExample<FitPresetBinRequest> GetExample()
	{
		return OpenApiExample.Create(
			"fitPresetBinRequest",
			"Fit Preset Bin Request",
			new FitPresetBinRequest()
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


internal class FitPresetBinResponseExamples : IMultipleOpenApiExamplesProvider<FitBinResponse>
{
	public IEnumerable<IOpenApiExample<FitBinResponse>> GetExamples()
	{
		yield return OpenApiExample.Create(
			"fitResponse",
			"Fit Response",
			"Example response when all items fit into the bin and no items are left unpacked.",
			FitExampleResponses.Fits("preset_bin_1"));

		yield return OpenApiExample.Create(
			"doesNotFitResponse",
			"Does Not Fit Response",
			"Example response when some items don't fit into the bin.",
			FitExampleResponses.DoesNotFit("preset_bin_1"));

		yield return OpenApiExample.Create(
			"earlyExitResponse",
			"Early Exit Response",
			"Example response when the early exit condition is met and the algorithm exits before starting.",
			FitExampleResponses.EarlyExit("preset_bin_1"));
	}
}
