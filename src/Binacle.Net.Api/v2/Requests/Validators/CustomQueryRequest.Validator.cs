using Binacle.Net.Api.Validators;
using FluentValidation;

namespace Binacle.Net.Api.v2.Requests.Validators;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

internal class CustomQueryRequestValidator : AbstractValidator<CustomQueryRequest>
{
	public CustomQueryRequestValidator()
	{
		RuleFor(x => x.Bins)
		   .NotNull()
		   .NotEmpty()
		   .WithMessage(Constants.Errors.Messages.IsRequired);

		RuleForEach(x => x.Bins).ChildRules(binValidator =>
		{
			binValidator.Include(new ItemWithDimensionsValidator());
			binValidator.Include(new ItemWithIDValidator());
		});

		RuleFor(x => x.Items)
			.NotNull()
			.NotEmpty()
			.WithMessage(Constants.Errors.Messages.IsRequired);

		RuleForEach(x => x.Items).ChildRules(itemValidator =>
		{
			itemValidator.Include(new ItemWithQuantityValidator());
			itemValidator.Include(new ItemWithDimensionsValidator());
			itemValidator.Include(new ItemWithIDValidator());
		});
	}
}
