using Binacle.Lib.Components.Abstractions.Models;
using FluentValidation;

namespace Binacle.Api.Components.Validators
{
    public class ItemWithDimensionsValidator : AbstractValidator<IWithReadOnlyDimensions<ushort>>
    {
        public ItemWithDimensionsValidator()
        {
            RuleFor(x => x.Length).NotNull().GreaterThan((ushort)0);
            RuleFor(x => x.Width).NotNull().GreaterThan((ushort)0);
            RuleFor(x => x.Height).NotNull().GreaterThan((ushort)0);
        }
    }
}
