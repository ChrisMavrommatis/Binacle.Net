using Binacle.Net.Kernel.OpenApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.v4.Contracts;

#pragma warning disable CS1591

public abstract class CustomBinRequestBase : IWithOperationParameters, IWithBin, IWithItems
{
	public required OperationParameters Parameters { get; set; }
	public required Bin Bin { get; set; }
	public required List<Box> Items { get; set; }
}



internal class CustomBinValidationProblemResponseExamples : IMultipleOpenApiExamplesProvider<ProblemDetails>
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
			"invalidBinData",
			"Invalid Bin Data",
			"Example response when you provide invalid Bin data",
			new Dictionary<string, string[]>()
			{
				{ "Bin.Length", ["'Length' must be greater than '0'."] }
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
