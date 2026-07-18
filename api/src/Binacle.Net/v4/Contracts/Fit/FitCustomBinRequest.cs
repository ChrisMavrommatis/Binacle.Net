using OpenApiExamples;
using OpenApiExamples.Abstractions;
using System.ComponentModel;

namespace Binacle.Net.v4.Contracts.Fit;

#pragma warning disable CS1591

[Description("A request to check whether items fit into a custom bin.")]
public class FitCustomBinRequest : CustomBinRequestBase;

internal class FitCustomBinRequestValidator : CustomBinRequestBaseValidator<FitCustomBinRequest>;

internal class FitCustomBinRequestExample : ISingleOpenApiExamplesProvider<FitCustomBinRequest>
{
	public IOpenApiExample<FitCustomBinRequest> GetExample()
	{
		return OpenApiExample.Create(
			"fitCustomBinRequest",
			"Fit Custom Bin Request",
			new FitCustomBinRequest()
			{
				Parameters = new OperationParameters()
				{
					Algorithm = Algorithm.Best,
					IncludeViPaqData = true,
				},
				Bin = ExampleData.SingleBin("custom_bin"),
				Items = ExampleData.Items()
			}
		);
	}
}

internal class FitCustomBinResponseExamples : IMultipleOpenApiExamplesProvider<FitBinResponse>
{
	public IEnumerable<IOpenApiExample<FitBinResponse>> GetExamples()
	{
		yield return OpenApiExample.Create(
			"fitResponse",
			"Fit Response",
			"Example response when all items fit into the bin and no items are left unpacked.",
			FitExampleResponses.Fits("custom_bin"));

		yield return OpenApiExample.Create(
			"doesNotFitResponse",
			"Does Not Fit Response",
			"Example response when some items don't fit into the bin.",
			FitExampleResponses.DoesNotFit("custom_bin"));

		yield return OpenApiExample.Create(
			"earlyExitResponse",
			"Early Exit Response",
			"Example response when the early exit condition is met and the algorithm exits before starting.",
			FitExampleResponses.EarlyExit("custom_bin"));
	}
}
