using Binacle.Net.Constants;
using Binacle.Net.Validators;
using FluentValidation;

namespace Binacle.Net.v3.Contracts;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public interface IWithBins
{
	List<Bin> Bins { get; set; }
}
internal class BinsValidator : AbstractValidator<IWithBins>
{
	public BinsValidator()
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
	}
}
