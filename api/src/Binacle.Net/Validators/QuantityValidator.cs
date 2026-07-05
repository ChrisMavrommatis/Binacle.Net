using Binacle.Geometry;
using FluentValidation;

namespace Binacle.Net.Validators;

internal class QuantityValidator : AbstractValidator<IWithQuantity<int>>
{
	public QuantityValidator()
	{
		RuleFor(x => x.Quantity)
			.NotNull()
			.GreaterThan(0)
			.LessThanOrEqualTo(ushort.MaxValue);
	}
}
