using Binacle.Net.Validators;
using FluentValidation;

namespace Binacle.Net.v4.Contracts;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public interface IWithBin
{
	Bin Bin { get; set; }
}

internal class BinValidator : AbstractValidator<IWithBin>
{
	public BinValidator()
	{
		RuleFor(x => x.Bin)
			.NotNull()
			.NotEmpty();
		
		RuleFor(x => x.Bin) .ChildRules(binValidator =>
		{
			binValidator.Include(new DimensionsValidator());
			binValidator.Include(new IDValidator());
		});
	}
}

