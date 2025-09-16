using System.Text.Json.Serialization;
using Binacle.Net.Kernel.Serialization;
using Binacle.Net.Models;
using FluentValidation;

namespace Binacle.Net.v3.Contracts;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public interface IWithFittingParameters
{
	FitRequestParameters Parameters { get; set; }
}

public class FitRequestParameters : IWithAlgorithm
{
	[JsonConverter(typeof(JsonStringNullableEnumConverter))]
	public required Algorithm? Algorithm { get; set; }
}

internal class FitRequestParametersValidator : AbstractValidator<IWithFittingParameters>
{

	public FitRequestParametersValidator()
	{
		RuleFor(x => x.Parameters)
			.NotNull();

		RuleFor(x => x.Parameters!)
			.ChildRules(parametersValidator =>
			{
				parametersValidator.Include(new AlgorithmValidator());
			});
	}
}
