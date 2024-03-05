using Binacle.Net.Api.Models.Requests;
using FluentValidation;

namespace Binacle.Net.Api.Validators;

public class PresetRequestValidator : AbstractValidator<PresetQueryRequest>
{
	public PresetRequestValidator()
	{
		RuleFor(x => x.Items)
			.NotNull()
			.NotEmpty()
			.WithMessage(Constants.ErrorMessages.IsRequired);

		RuleForEach(x => x.Items).ChildRules(itemValidator =>
		{
			itemValidator.Include(new ItemWithQuantityValidator());
			itemValidator.Include(new ItemWithDimensionsValidator());
			itemValidator.Include(new ItemWithIDValidator());
		});
	}
}
