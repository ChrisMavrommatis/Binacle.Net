using Binacle.Net.Api.DiagnosticsModule.Configuration.Models;
using Binacle.Net.Api.DiagnosticsModule.Models;
using FluentValidation;

namespace Binacle.Net.Api.DiagnosticsModule.Configuration.Validators;

internal class HealthCheckConfigurationOptionsValidator : AbstractValidator<HealthCheckConfigurationOptions>
{
	public HealthCheckConfigurationOptionsValidator()
	{
		RuleFor(x => x.Path).NotNull().NotEmpty();
		RuleFor(x => x.Path).Must(x => x.StartsWith("/")).WithMessage("Path must start with /");
		RuleForEach(x => x.RestrictedIPs)
			.ChildRules(childRule => 
			{
				childRule.RuleFor(x => x).Must(x => IPAddressRange.ParseRange(x) != null).WithMessage("Invalid IP Address Range");
			});
	}
}
