using Binacle.Net.Api.Validators;
using FluentValidation;

namespace Binacle.Net.Api.Models.Requests.Validators;

public class CustomQueryRequestValidator : AbstractValidator<CustomQueryRequest>
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
