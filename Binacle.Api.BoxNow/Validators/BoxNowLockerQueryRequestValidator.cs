using Binacle.Api.BoxNow.Requests;
using FluentValidation;

namespace Binacle.Api.BoxNow.Validators
{
    public class BoxNowLockerQueryRequestValidator : AbstractValidator<BoxNowLockerQueryRequest>
    {
        public BoxNowLockerQueryRequestValidator()
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
