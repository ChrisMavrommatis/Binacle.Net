using Binacle.Net.Constants;
using Binacle.Net.Validators;
using FluentValidation;

namespace Binacle.Net.v1.Requests.Validators;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

internal class CustomQueryRequestValidator : AbstractValidator<CustomQueryRequest>
{
	public CustomQueryRequestValidator()
	{
		RuleFor(x => x.Bins)
		   .NotNull()
		   .NotEmpty()
		   .WithMessage(ErrorMessage.IsRequired);

		// Each Bin Id must be unique
		RuleFor(x => x.Bins)
			.Must(x => x!.Select(y => y.ID).Distinct().Count() == x!.Count)
			.WithMessage(ErrorMessage.IdMustBeUnique);


		RuleForEach(x => x.Bins).ChildRules(binValidator =>
		{
			binValidator.Include(new DimensionsValidator());
			binValidator.Include(new IDValidator());
		});

		RuleFor(x => x.Items)
			.NotNull()
			.NotEmpty()
			.WithMessage(ErrorMessage.IsRequired);

		// Each ItemID must be unique
		RuleFor(x => x.Items)
			.Must(x => x!.Select(y => y.ID).Distinct().Count() == x!.Count)
			.WithMessage(ErrorMessage.IdMustBeUnique);

		RuleForEach(x => x.Items).ChildRules(itemValidator =>
		{
			itemValidator.Include(new QuantityValidator());
			itemValidator.Include(new DimensionsValidator());
			itemValidator.Include(new IDValidator());
		});
	}
}
