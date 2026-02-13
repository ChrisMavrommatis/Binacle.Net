using Binacle.Net.Validators;
using FluentValidation;

namespace Binacle.Net.v4.Contracts;

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
			.NotEmpty();

		// Each ItemID must be unique
		RuleFor(x => x.Items)
			.Must(x => x!.Select(y => y.ID).Distinct().Count() == x!.Count)
			.WithMessage("IDs in `Items` must be unique");

		RuleForEach(x => x.Items).ChildRules(itemValidator =>
		{
			itemValidator.Include(new QuantityValidator());
			itemValidator.Include(new DimensionsValidator());
			itemValidator.Include(new IDValidator());
			itemValidator.RuleFor(item => item)
				.MustNotThrow(item =>
				{
					var volume = checked((item.Length * item.Width * item.Height) * item.Quantity);
				})
				.WithMessage("The total volume of the item results in a number that the api cannot handle.");
		});

		RuleFor(x => x.Items)
			.MustNotThrow(x =>
			{
				var volume = x.Sum(item => (item.Length * item.Width * item.Height) * item.Quantity);
			})
			.WithMessage("The total volume of all the items results in a number that the api cannot handle.");
	}
}
