using System.Text.Json.Serialization;
using Binacle.Net.Kernel.Serialization;
using FluentValidation;

namespace Binacle.Net.v4.Contracts;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public interface IWithOperationParameters
{
	OperationParameters Parameters { get; set; }
}

public class OperationParameters : IWithAlgorithm
{
	[JsonConverter(typeof(JsonStringNullableEnumConverter))]
	public required Algorithm? Algorithm { get; set; }

	public bool IncludeViPaqData { get; set; } = false;
}

internal class OperationParametersValidator : AbstractValidator<IWithOperationParameters>
{

	public OperationParametersValidator()
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
