using FluentValidation;

namespace Binacle.Api.BoxNow.Configuration
{
    public class BoxNowOptionsValidator: AbstractValidator<BoxNowOptions>
    {
        public BoxNowOptionsValidator()
        {
            RuleFor(x => x.Lockers)
                .NotEmpty();

            RuleForEach(x => x.Lockers).ChildRules(validator =>
            {
                validator.RuleFor(x => x.Size).GreaterThan(0).WithMessage($"Size in Locker must be greater than 0");
                validator.RuleFor(x => x.Height).GreaterThan(0).WithMessage($"Height in Locker must be greater than 0");
                validator.RuleFor(x => x.Width).GreaterThan(0).WithMessage($"Width in Locker must be greater than 0");
                validator.RuleFor(x => x.Length).GreaterThan(0).WithMessage($"Length in Locker must be greater than 0");
            });
                
        }
    }
}
