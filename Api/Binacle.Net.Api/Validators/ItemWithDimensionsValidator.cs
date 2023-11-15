using Binacle.Net.Lib.Abstractions.Models;
using FluentValidation;

namespace Binacle.Net.Api.Validators
{
    public class ItemWithDimensionsValidator : AbstractValidator<IWithReadOnlyDimensions<int>>
    {
        public ItemWithDimensionsValidator()
        {
            RuleFor(x => x.Length).NotNull().WithMessage(Constants.ErrorMessages.IsRequired);
            RuleFor(x => x.Width).NotNull().WithMessage(Constants.ErrorMessages.IsRequired);
            RuleFor(x => x.Height).NotNull().WithMessage(Constants.ErrorMessages.IsRequired);

            RuleFor(x => x.Length).GreaterThan(0).WithMessage(Constants.ErrorMessages.GreaterThanZero);
            RuleFor(x => x.Width).GreaterThan(0).WithMessage(Constants.ErrorMessages.GreaterThanZero);
            RuleFor(x => x.Height).GreaterThan(0).WithMessage(Constants.ErrorMessages.GreaterThanZero);
        }
    }
}
