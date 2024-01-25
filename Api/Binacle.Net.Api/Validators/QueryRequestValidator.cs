using Binacle.Net.Api.Models.Requests;
using FluentValidation;

namespace Binacle.Net.Api.Validators;

public class QueryRequestValidator : AbstractValidator<QueryRequest>
{
    public QueryRequestValidator()
    {
        RuleFor(x => x.Containers)
           .NotNull()
           .NotEmpty()
           .WithMessage(Constants.ErrorMessages.IsRequired);

        RuleForEach(x => x.Containers).ChildRules(containerValidator =>
        {
            containerValidator.Include(new ItemWithDimensionsValidator());
            containerValidator.Include(new ItemWithIDValidator());
        });

        RuleFor(x => x.Items)
            .NotNull()
            .NotEmpty()
            .WithMessage(Constants.ErrorMessages.IsRequired);

        RuleForEach(x => x.Items).ChildRules(itemValidator =>
        {
            itemValidator.Include(new ItemWithQuantityValidator());
            itemValidator.Include(new ItemWithDimensionsValidator());
            itemValidator.Include(new ItemWithIDValidator());
        });
    }
}
