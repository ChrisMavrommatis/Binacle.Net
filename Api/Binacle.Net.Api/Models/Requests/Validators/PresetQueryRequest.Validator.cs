using Binacle.Net.Api.Validators;
using FluentValidation;

namespace Binacle.Net.Api.Models.Requests.Validators;

public class PresetQueryRequestValidator : AbstractValidator<PresetQueryRequest>
{
	public PresetQueryRequestValidator()
	{
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
