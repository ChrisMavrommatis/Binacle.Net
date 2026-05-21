using FluentValidation;

namespace Binacle.Net.v4.Contracts;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public interface IWithAlgorithm
{
	Algorithm? Algorithm { get; }
}


public enum Algorithm
{
	FFD,
	WFD,
	BFD,
	Best
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
