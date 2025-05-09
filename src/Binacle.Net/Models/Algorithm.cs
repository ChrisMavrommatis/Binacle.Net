using FluentValidation;

namespace Binacle.Net.Models;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public interface IWithAlgorithm
{
	Algorithm? Algorithm { get; }
}

public enum Algorithm
{
	FFD,
	WFD,
	BFD
}

internal class AlgorithmValidator : AbstractValidator<IWithAlgorithm>
{
	public AlgorithmValidator()
	{
		var enumValues = Enum.GetValues<Algorithm>();
		
		RuleFor(x => x.Algorithm)
			.NotNull()
			.WithMessage(ErrorMessage.RequiredEnumValues<Algorithm>(nameof(IWithAlgorithm.Algorithm)));
	}
}
