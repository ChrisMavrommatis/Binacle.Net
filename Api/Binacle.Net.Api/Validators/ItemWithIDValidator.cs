using Binacle.Net.Lib.Abstractions.Models;
using FluentValidation;

namespace Binacle.Net.Api.Validators;

public class ItemWithIDValidator : AbstractValidator<IWithID>
{
    public ItemWithIDValidator()
    {
        RuleFor(x => x.ID).NotNull().NotEmpty().WithMessage(Constants.ErrorMessages.IsRequired);
    }
}
