using Binacle.Net.Lib.Abstractions.Models;
using FluentValidation;

namespace Binacle.Net.Api.Validators
{
    public class ItemWithQuantityValidator: AbstractValidator<IWithQuantity<int>>
    {
        public ItemWithQuantityValidator()
        {
            RuleFor(x => x.Quantity).NotNull().WithMessage(Constants.ErrorMessages.IsRequired);
            RuleFor(x => x.Quantity).LessThanOrEqualTo(ushort.MaxValue).WithMessage(Constants.ErrorMessages.LessThanUShortMaxValue);
            RuleFor(x => x.Quantity).GreaterThan(ushort.MinValue).WithMessage(Constants.ErrorMessages.GreaterThanZero);
        }
    }
}
