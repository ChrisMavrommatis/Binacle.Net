using Binacle.Lib.Components.Models;
using FluentValidation;

namespace Binacle.Api.Components.Validators
{
    public class ItemWithDimensionsValidator : AbstractValidator<IWithDimensions>
    {
        public ItemWithDimensionsValidator()
        {
            RuleFor(x => x.Length).NotNull().GreaterThan(0);
            RuleFor(x => x.Width).NotNull().GreaterThan(0);
            RuleFor(x => x.Height).NotNull().GreaterThan(0);
        }
    }
}
