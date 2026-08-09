using System.ComponentModel;
using FluentValidation;

namespace Binacle.Net.v3.Contracts;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public interface IWithAlgorithm
{
	Algorithm? Algorithm { get; }
}

[Description("The packing heuristic to use.")]
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
		
		RuleFor(x => x.Algorithm)
			.NotNull()
			.WithMessage(ErrorMessage.RequiredEnumValues<Algorithm>(nameof(IWithAlgorithm.Algorithm)));
	}
}
