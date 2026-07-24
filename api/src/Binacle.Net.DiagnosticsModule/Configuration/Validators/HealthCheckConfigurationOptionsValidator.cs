using Binacle.Net.DiagnosticsModule.Configuration.Models;
using Binacle.Net.DiagnosticsModule.Models;
using FluentValidation;

namespace Binacle.Net.DiagnosticsModule.Configuration.Validators;

internal class HealthCheckConfigurationOptionsValidator : AbstractValidator<HealthCheckConfigurationOptions>
{
	public HealthCheckConfigurationOptionsValidator()
	{
		// Cascade(Stop) is load-bearing, not tidying. Without it every rule in the chain runs even after one fails,
		// so a null was reported twice and then dereferenced by the predicate — crashing the start this exists to
		// explain. Stopping at the first failure gives one message and lets the predicate assume a value.
		RuleFor(x => x.Path)
			.Cascade(CascadeMode.Stop)
			.NotEmpty()
			.Must(path => path!.StartsWith("/"))
			.WithMessage("'{PropertyName}' must start with '/', for example '/_health'.");
		RuleForEach(x => x.RestrictedIPs)
			.ChildRules(childRule =>
			{
				childRule.RuleFor(x => x)
					.Must(x => RestrictedIPNetwork.TryParse(x, out _))
					.WithMessage(
						"'{PropertyValue}' is not a valid entry. Use a single address such as 192.168.1.1, or CIDR " +
						"notation such as 192.168.1.0/24, where the number is a prefix length. Address ranges " +
						"(1.2.3.4-1.2.3.9) are no longer supported."
					);
			});
	}
}
