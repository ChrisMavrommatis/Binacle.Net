using Binacle.Lib.Components.Models;
using FluentValidation;

namespace Binacle.Api.Components.Validators
{
    public class ItemWithIDValidator : AbstractValidator<IWithID>
    {
        public ItemWithIDValidator()
        {
            RuleFor(x => x.ID).NotNull().NotEmpty();
        }
    }
}
