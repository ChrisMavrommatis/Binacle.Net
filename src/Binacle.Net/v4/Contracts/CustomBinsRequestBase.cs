using Binacle.Net.Kernel.OpenApi.Helpers;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.v4.Contracts;

#pragma warning disable CS1591

public abstract class CustomBinsRequestBase : IWithOperationParameters, IWithBins, IWithItems
{
	public required OperationParameters Parameters { get; set; }
	public required List<Bin> Bins { get; set; }
	public required List<Box> Items { get; set; }
}

internal class CustomBinsRequestBaseValidator : AbstractValidator<CustomBinsRequestBase>
{
	public CustomBinsRequestBaseValidator()
	{
		Include(new OperationParametersValidator());
		Include(new BinsValidator());
		Include(new ItemsValidator());
	}
}



internal class CustomBinsValidationProblemResponseExamples : IMultipleOpenApiExamplesProvider<ProblemDetails>
{
	public IEnumerable<IOpenApiExample<ProblemDetails>> GetExamples()
	{
		yield return OpenApiValidationProblemExample.Create(
			"invalidAlgorithm",
			"Invalid Algorithm",
			"Example response when you provide invalid algorithm",
			new Dictionary<string, string[]>()
			{
				{ "Parameters.Algorithm", [ErrorMessage.RequiredEnumValues<Algorithm>(nameof(IWithAlgorithm.Algorithm))] }
			}
		);
		
		yield return OpenApiValidationProblemExample.Create(
			"ivalidBinData",
			"Invalid Bin Data",
			"Example response when you provide invalid Bin data",
			new Dictionary<string, string[]>()
			{
				{ "Bins", ["IDs in `Bins` must be unique"] },
				{ "Bins[0].Length", ["'Length' must be greater than '0'."] }
			}
		);

		yield return OpenApiValidationProblemExample.Create(
			"invalidItemData",
			"Invalid Item Data",
			"Example response when you provide invalid Item data",
			new Dictionary<string, string[]>()
			{
				{ "Items[1].Length", ["'Length' must be less than or equal to '65535'."] }
			}
		);
	}
}
