using Binacle.Net.DiagnosticsModule.Configuration.Models;
using Binacle.Net.DiagnosticsModule.Models;
using FluentValidation;

namespace Binacle.Net.DiagnosticsModule.Configuration.Validators;

internal class HealthCheckConfigurationOptionsValidator : AbstractValidator<HealthCheckConfigurationOptions>
{
	public HealthCheckConfigurationOptionsValidator()
	{
		RuleFor(x => x.Path).NotNull().NotEmpty();
		RuleFor(x => x.Path).Must(x => x!.StartsWith("/")).WithMessage("Path must start with /");
		RuleForEach(x => x.RestrictedIPs)
			.ChildRules(childRule =>
			{
				childRule.RuleFor(x => x)
					.Must(x => RestrictedIPNetwork.TryParse(x, out _))
					.WithMessage(
						"Invalid entry. Use a single address such as 192.168.1.1, or CIDR notation such as 192.168.1.0/24"
					);
			});
	}
}
