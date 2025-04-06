using Binacle.Net.Constants;
using Binacle.Lib.Abstractions.Models;
using FluentValidation;

namespace Binacle.Net.Validators;

internal class ItemWithQuantityValidator : AbstractValidator<IWithQuantity<int>>
{
	public ItemWithQuantityValidator()
	{
		RuleFor(x => x.Quantity).NotNull().WithMessage(ErrorMessage.IsRequired);
		RuleFor(x => x.Quantity).GreaterThan(0).WithMessage(ErrorMessage.GreaterThanZero);
		RuleFor(x => x.Quantity).LessThanOrEqualTo(ushort.MaxValue).WithMessage(ErrorMessage.LessThanUShortMaxValue);
	}
}
