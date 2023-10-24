using Binacle.Api.Components.Validators;
using Binacle.Api.Models;
using FluentValidation;

namespace Binacle.Api.Validators
{
    public class QueryRequestValidator : AbstractValidator<QueryRequest>
    {
        public QueryRequestValidator()
        {
            RuleFor(x => x.Containers)
               .NotEmpty();

            RuleForEach(x => x.Containers).ChildRules(v =>
            {
                v.Include(new ItemWithDimensionsValidator());
                v.Include(new ItemWithIDValidator());
            });

            RuleFor(x => x.Items)
                .NotEmpty();

            RuleForEach(x => x.Items).ChildRules(v =>
            {
                v.RuleFor(x => x.Quantity).NotNull().GreaterThan(0);
                v.Include(new ItemWithDimensionsValidator());
                v.Include(new ItemWithIDValidator());
            });
        }
    }
}
