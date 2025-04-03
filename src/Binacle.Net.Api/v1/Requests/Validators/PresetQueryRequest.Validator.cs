using Binacle.Net.Api.Constants;
using Binacle.Net.Api.Validators;
using FluentValidation;

namespace Binacle.Net.Api.v1.Requests.Validators;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

internal class PresetQueryRequestValidator : AbstractValidator<PresetQueryRequest>
{
	public PresetQueryRequestValidator()
	{
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
			itemValidator.Include(new ItemWithQuantityValidator());
			itemValidator.Include(new ItemWithDimensionsValidator());
			itemValidator.Include(new ItemWithIDValidator());
		});
	}
}
