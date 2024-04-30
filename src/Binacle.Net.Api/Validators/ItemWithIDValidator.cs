using Binacle.Net.Lib.Abstractions.Models;
using FluentValidation;

namespace Binacle.Net.Api.Validators;

internal class ItemWithIDValidator : AbstractValidator<IWithID>
{
	public ItemWithIDValidator()
	{
		RuleFor(x => x.ID).NotNull().NotEmpty().WithMessage(Constants.Errors.Messages.IsRequired);
	}
}
