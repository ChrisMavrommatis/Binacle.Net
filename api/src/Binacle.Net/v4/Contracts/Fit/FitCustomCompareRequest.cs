using FluentValidation;
using OpenApiExamples;
using OpenApiExamples.Abstractions;
using System.ComponentModel;

namespace Binacle.Net.v4.Contracts.Fit;

#pragma warning disable CS1591

[Description("A request to check whether items fit into each of several custom bins.")]
public class FitCustomCompareRequest : CustomBinsRequestBase;

internal class FitCustomCompareRequestValidator : AbstractValidator<FitCustomCompareRequest>
{
	public FitCustomCompareRequestValidator()
	{
		Include(new CustomBinsRequestBaseValidator());
	}
}

internal class FitCustomCompareRequestExample : ISingleOpenApiExamplesProvider<FitCustomCompareRequest>
{
	public IOpenApiExample<FitCustomCompareRequest> GetExample()
	{
		return OpenApiExample.Create(
			"fitCustomCompareRequest",
			"Fit Custom Compare Request",
			new FitCustomCompareRequest()
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

internal class FitCustomCompareResponseExamples : IMultipleOpenApiExamplesProvider<FitCompareResponse>
{
	public IEnumerable<IOpenApiExample<FitCompareResponse>> GetExamples()
	{
		yield return OpenApiExample.Create(
			"compareResponse",
			"Compare Response",
			"Example response with one result per requested bin, in the order the bins were sent.",
			new FitCompareResponse
			{
				Results = ExampleData.Bins("custom_bin")
					.Select(bin => FitExampleResponses.Fits(bin))
					.ToList()
			}
		);
	}
}
