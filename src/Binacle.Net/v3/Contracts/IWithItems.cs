using Binacle.Net.Constants;
using Binacle.Net.Validators;
using FluentValidation;

namespace Binacle.Net.v3.Contracts;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public interface IWithItems
{
	List<Box> Items { get; set; }
}

internal class ItemsValidator : AbstractValidator<IWithItems>
{
	public ItemsValidator()
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
			itemValidator.Include(new QuantityValidator());
			itemValidator.Include(new DimensionsValidator());
			itemValidator.Include(new IDValidator());
		});
	}
}
