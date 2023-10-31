using Binacle.Net.Api.Options.Models;
using FluentValidation;

namespace Binacle.Net.Api.Options.Validators
{
    public class BinPresetOptionsOptionsValidator : AbstractValidator<BinPresetOptions>
    {
        public BinPresetOptionsOptionsValidator()
        {
            RuleFor(x => x.Sample)
                .NotNull();

            RuleFor(x => x.Sample.Bins)
                .NotNull()
                .NotEmpty();

            RuleForEach(x => x.Sample.Bins).ChildRules(validator =>
            {
                validator.RuleFor(x => x.Size).GreaterThan(0).WithMessage($"Size in Bin must be greater than 0");
                validator.RuleFor(x => x.Height).GreaterThan(0).WithMessage($"Height in Bin must be greater than 0");
                validator.RuleFor(x => x.Width).GreaterThan(0).WithMessage($"Width in Bin must be greater than 0");
                validator.RuleFor(x => x.Length).GreaterThan(0).WithMessage($"Length in LockBiner must be greater than 0");
            });

        }
    }
}
