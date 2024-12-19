using Binacle.Net.Api.Models;
using FluentValidation;

namespace Binacle.Net.Api.Validators;

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
