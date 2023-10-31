using Binacle.Net.Lib.Abstractions.Models;
using FluentValidation;

namespace Binacle.Net.Api.Validators
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
