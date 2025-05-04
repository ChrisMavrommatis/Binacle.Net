using Binacle.Net.Constants;
using Binacle.Lib.Abstractions.Models;
using FluentValidation;

namespace Binacle.Net.Validators;

internal class DimensionsValidator : AbstractValidator<IWithReadOnlyDimensions<int>>
{
	public DimensionsValidator()
	{
		RuleFor(x => x).Must(x => x.Height * x.Width * x.Length > -1).WithMessage(ErrorMessage.VolumeOverflow);
		RuleFor(x => x.Length).NotNull().WithMessage(ErrorMessage.IsRequired);
		RuleFor(x => x.Width).NotNull().WithMessage(ErrorMessage.IsRequired);
		RuleFor(x => x.Height).NotNull().WithMessage(ErrorMessage.IsRequired);

		RuleFor(x => x.Length).GreaterThan(0).WithMessage(ErrorMessage.GreaterThanZero);
		RuleFor(x => x.Width).GreaterThan(0).WithMessage(ErrorMessage.GreaterThanZero);
		RuleFor(x => x.Height).GreaterThan(0).WithMessage(ErrorMessage.GreaterThanZero);

		RuleFor(x => x.Length).LessThanOrEqualTo(ushort.MaxValue).WithMessage(ErrorMessage.LessThanUShortMaxValue);
		RuleFor(x => x.Width).LessThanOrEqualTo(ushort.MaxValue).WithMessage(ErrorMessage.LessThanUShortMaxValue);
		RuleFor(x => x.Height).LessThanOrEqualTo(ushort.MaxValue).WithMessage(ErrorMessage.LessThanUShortMaxValue);
	}
}
