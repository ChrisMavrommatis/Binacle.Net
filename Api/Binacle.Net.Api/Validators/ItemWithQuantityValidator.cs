using Binacle.Net.Lib.Abstractions.Models;
using FluentValidation;

namespace Binacle.Net.Api.Validators;

public class ItemWithQuantityValidator : AbstractValidator<IWithQuantity<int>>
{
	public ItemWithQuantityValidator()
	{
		RuleFor(x => x.Quantity).NotNull().WithMessage(Constants.Errors.Messages.IsRequired);
		RuleFor(x => x.Quantity).GreaterThan(0).WithMessage(Constants.Errors.Messages.GreaterThanZero);
		RuleFor(x => x.Quantity).LessThanOrEqualTo(ushort.MaxValue).WithMessage(Constants.Errors.Messages.LessThanUShortMaxValue);
	}
}
