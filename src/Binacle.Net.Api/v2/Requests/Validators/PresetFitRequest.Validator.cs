using Binacle.Net.Api.Validators;
using FluentValidation;

namespace Binacle.Net.Api.v2.Requests.Validators;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

internal class PresetFitRequestValidator : AbstractValidator<PresetFitRequest>
{
	public PresetFitRequestValidator()
	{
		RuleFor(x => x.Items)
			.NotNull()
			.NotEmpty()
			.WithMessage(Constants.Errors.Messages.IsRequired);

		// Each ItemID must be unique
		RuleFor(x => x.Items)
			.Must(x => x!.Select(y => y.ID).Distinct().Count() == x!.Count)
			.WithMessage(Constants.Errors.Messages.IdMustBeUnique);

		RuleForEach(x => x.Items).ChildRules(itemValidator =>
		{
			itemValidator.Include(new ItemWithQuantityValidator());
			itemValidator.Include(new ItemWithDimensionsValidator());
			itemValidator.Include(new ItemWithIDValidator());
		});
	}
}
