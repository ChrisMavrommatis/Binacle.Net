using Binacle.Lib.Abstractions.Models;
using FluentValidation;

namespace Binacle.Net.Validators;

internal class DimensionsValidator : AbstractValidator<IWithReadOnlyDimensions<int>>
{
	public DimensionsValidator()
	{
		RuleFor(x => x)
			.MustNotThrow(x =>
			{
				var volume = checked(x.Length * x.Width * x.Height);
			})
			.WithMessage("The volume results in a number that the api cannot handle.");
		
		RuleFor(x => x.Length)
			.NotNull()
			.GreaterThan(0)
			.LessThanOrEqualTo(ushort.MaxValue);

		RuleFor(x => x.Width)
			.NotNull()
			.GreaterThan(0)
			.LessThanOrEqualTo(ushort.MaxValue);
		
		RuleFor(x => x.Height)
			.NotNull()
			.GreaterThan(0)
			.LessThanOrEqualTo(ushort.MaxValue);
	}
}
