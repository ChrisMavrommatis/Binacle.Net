using FluentValidation;
using OpenApiExamples;
using OpenApiExamples.Abstractions;
using System.ComponentModel;

namespace Binacle.Net.v4.Contracts.Pack;

#pragma warning disable CS1591

[Description("A request to pack items into each of several custom bins.")]
public class PackCustomCompareRequest : CustomBinsRequestBase;

internal class PackCustomCompareRequestValidator : AbstractValidator<PackCustomCompareRequest>
{
	public PackCustomCompareRequestValidator()
	{
		Include(new CustomBinsRequestBaseValidator());
	}
}

internal class PackCustomCompareRequestExample : ISingleOpenApiExamplesProvider<PackCustomCompareRequest>
{
	public IOpenApiExample<PackCustomCompareRequest> GetExample()
	{
		return OpenApiExample.Create(
			"packCustomCompareRequest",
			"Pack Custom Compare Request",
			new PackCustomCompareRequest()
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

internal class PackCustomCompareResponseExamples : IMultipleOpenApiExamplesProvider<PackCompareResponse>
{
	public IEnumerable<IOpenApiExample<PackCompareResponse>> GetExamples()
	{
		yield return OpenApiExample.Create(
			"compareResponse",
			"Compare Response",
			"Example response with one result per requested bin, in the order the bins were sent.",
			new PackCompareResponse
			{
				Results = ExampleData.Bins("custom_bin")
					.Select(bin => PackExampleResponses.FullyPacked(bin))
					.ToList()
			}
		);
	}
}
