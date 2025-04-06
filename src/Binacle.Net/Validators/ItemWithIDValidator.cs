using Binacle.Net.Constants;
using Binacle.Lib.Abstractions.Models;
using FluentValidation;

namespace Binacle.Net.Validators;

internal class ItemWithIDValidator : AbstractValidator<IWithID>
{
	public ItemWithIDValidator()
	{
		RuleFor(x => x.ID).NotNull().NotEmpty().WithMessage(ErrorMessage.IsRequired);
	}
}
