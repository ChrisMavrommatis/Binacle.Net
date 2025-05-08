using Binacle.Lib.Abstractions.Models;
using FluentValidation;

namespace Binacle.Net.Validators;

internal class IDValidator : AbstractValidator<IWithID>
{
	public IDValidator()
	{
		RuleFor(x => x.ID)
			.NotNull()
			.NotEmpty();
	}
}
