using Binacle.Net.Api.DiagnosticsModule.Configuration.Models;
using FluentValidation;

namespace Binacle.Net.Api.DiagnosticsModule.Configuration.Validators;

internal class PackingLogsConfigurationOptionsValidator : AbstractValidator<PackingLogsConfigurationOptions>
{
	public PackingLogsConfigurationOptionsValidator()
	{
		When(x => x.Enabled, () =>
		{
			RuleFor(x => x.LegacyPacking)
				.NotNull()
				.ChildRules(x => x.Include(new PackingLogOptionsValidator()));
			
			RuleFor(x => x.LegacyFitting)
				.NotNull()
				.ChildRules(x => x.Include(new PackingLogOptionsValidator()));

			RuleFor(x => x.Packing)
				.NotNull()
				.ChildRules(x => x.Include(new PackingLogOptionsValidator()));
		});
	}
}

internal class PackingLogOptionsValidator : AbstractValidator<PackingLogOptions?>
{
	public PackingLogOptionsValidator()
	{
		When(x => x is not null, () =>
		{
			RuleFor(x => x!.Path).NotNull().NotEmpty();
			RuleFor(x => x!.FileName)
				.NotNull()
				.NotEmpty()
				.Must(x => x!.Contains("{0}"));
		});
			
	}
}
