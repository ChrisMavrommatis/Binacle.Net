using Binacle.Net.Kernel.OpenApi.Helpers;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OpenApiExamples.Abstractions;
using System.ComponentModel;

namespace Binacle.Net.v4.Contracts;

#pragma warning disable CS1591
public abstract class PresetBinsRequestBase : IWithOperationParameters, IWithItems
{
	[Description(SchemaDescriptions.Parameters)]
	public required OperationParameters Parameters { get; set; }
	
	[Description(SchemaDescriptions.Items)]
	public required List<Box> Items { get; set; }
}

internal abstract class PresetBinsRequestBaseValidator<T> : AbstractValidator<T>
	where T : PresetBinsRequestBase
{
	protected PresetBinsRequestBaseValidator()
	{
		Include(new OperationParametersValidator());
		Include(new ItemsValidator());
	}
}

internal class PresetBinsValidationProblemResponseExamples : IMultipleOpenApiExamplesProvider<ProblemDetails>
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
