using Binacle.Api.Glockers.Models;
using FluentValidation;

namespace Binacle.Api.Glockers.Validators
{
    public class GlockersQueryRequestValidator : AbstractValidator<GlockersQueryRequest>
    {
        public GlockersQueryRequestValidator()
        {
            RuleFor(x => x.Items)
                .NotEmpty();

            RuleForEach(x => x.Items).ChildRules(v =>
            {
                v.RuleFor(x => x.ID).NotNull().NotEmpty();
                v.RuleFor(x => x.Length).NotNull().GreaterThan(0);
                v.RuleFor(x => x.Width).NotNull().GreaterThan(0);
                v.RuleFor(x => x.Height).NotNull().GreaterThan(0);
            });
        }
    }
}
