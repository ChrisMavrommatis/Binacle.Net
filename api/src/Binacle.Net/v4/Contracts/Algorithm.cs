using FluentValidation;
using System.ComponentModel;

namespace Binacle.Net.v4.Contracts;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public interface IWithAlgorithm
{
	Algorithm? Algorithm { get; }
}


[Description("The packing heuristic. FFD, WFD and BFD are the individual heuristics; Best runs more than one and "
            + "keeps the best result - all three on fit/bin and pack/bin, FFD plus BFD on every other route.")]
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
		
		RuleFor(x => x.Algorithm)
			.NotNull()
			.WithMessage(ErrorMessage.RequiredEnumValues<Algorithm>(nameof(IWithAlgorithm.Algorithm)));
	}
}
