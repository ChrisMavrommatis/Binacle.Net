using FluentValidation;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.v4.Contracts.Fit;

#pragma warning disable CS1591

public class FitCustomSmallestBinRequest : CustomBinsRequestBase;

internal class FitCustomSmallestBinRequestValidator : AbstractValidator<FitCustomSmallestBinRequest>
{
	public FitCustomSmallestBinRequestValidator()
	{
		Include(new CustomBinsRequestBaseValidator());
	}
}

internal class FitCustomSmallestBinRequestExample : ISingleOpenApiExamplesProvider<FitCustomSmallestBinRequest>
{
	public IOpenApiExample<FitCustomSmallestBinRequest> GetExample()
	{
		return OpenApiExample.Create(
			"fitCustomSmallestBinRequest",
			"Fit Custom Smallest Bin Request",
			new FitCustomSmallestBinRequest()
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

internal class FitCustomSmallestBinResponseExamples : IMultipleOpenApiExamplesProvider<FitBinResponse>
{
	public IEnumerable<IOpenApiExample<FitBinResponse>> GetExamples()
	{
		yield return OpenApiExample.Create(
			"fitResponse",
			"Fit Response",
			"Example response for the smallest bin the items fit into.",
			FitExampleResponses.Fits("custom_bin_1"));

		yield return OpenApiExample.Create(
			"doesNotFitResponse",
			"Does Not Fit Response",
			"Example response when the items don't fit into any of the bins.",
			FitExampleResponses.DoesNotFit("custom_bin_1"));
	}
}
