using Binacle.Net.Models;
using FluentValidation;

namespace Binacle.Net.Validators;

internal class AlgorithmValidator : AbstractValidator<IWithAlgorithm>
{
	public AlgorithmValidator()
	{
		var enumValues = Enum.GetValues<Algorithm>();
		
		RuleFor(x => x.Algorithm)
			.NotNull()
			.WithMessage($"Is required and must be one of the following values: {string.Join(", ", enumValues)}");
	}
}
