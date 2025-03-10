using Binacle.Net.Api.DiagnosticsModule.Configuration.Models;
using FluentValidation;

namespace Binacle.Net.Api.DiagnosticsModule.Configuration.Validators;

internal class AzureMonitorConfigurationOptionsValidator : AbstractValidator<AzureMonitorConfigurationOptions?>
{
	public AzureMonitorConfigurationOptionsValidator()
	{
		When(x => x is not null, () =>
		{
			RuleFor(x => x!.SamplingRatio)
				.InclusiveBetween(0.1f, 1.0f);
		});
	}
}
