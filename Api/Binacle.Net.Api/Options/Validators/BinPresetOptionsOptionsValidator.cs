using Binacle.Net.Api.Options.Models;
using FluentValidation;

namespace Binacle.Net.Api.Options.Validators
{
    public class BinPresetOptionsOptionsValidator : AbstractValidator<BinPresetOptions>
    {
        public BinPresetOptionsOptionsValidator()
        {
            RuleFor(x => x.Presets)
                .NotNull()
                .NotEmpty();


            RuleForEach(x => x.Presets).ChildRules(presetValidator =>
            {
                presetValidator.RuleFor(x => x.Value.Bins)
                    .NotNull()
                    .NotEmpty();

                presetValidator.RuleForEach(x => x.Value.Bins).ChildRules(binValidator =>
                {
                    binValidator.RuleFor(x => x.Height).GreaterThan(0).WithMessage($"Height in Bin must be greater than 0");
                    binValidator.RuleFor(x => x.Width).GreaterThan(0).WithMessage($"Width in Bin must be greater than 0");
                    binValidator.RuleFor(x => x.Length).GreaterThan(0).WithMessage($"Length in Bin must be greater than 0");
                });
            });

        }
    }
}
