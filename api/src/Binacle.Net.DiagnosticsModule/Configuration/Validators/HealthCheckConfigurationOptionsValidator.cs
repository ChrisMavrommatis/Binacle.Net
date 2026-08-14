using Binacle.Net.DiagnosticsModule.Configuration.Models;
using Binacle.Net.Kernel.Network;
using FluentValidation;

namespace Binacle.Net.DiagnosticsModule.Configuration.Validators;

internal class HealthCheckConfigurationOptionsValidator : AbstractValidator<HealthCheckConfigurationOptions>
{
	public HealthCheckConfigurationOptionsValidator()
	{
		// Without Cascade(Stop) every rule in the chain runs after one fails, so a null is reported twice and
		// then dereferenced by the predicate.
		RuleFor(x => x.Path)
			.Cascade(CascadeMode.Stop)
			.NotEmpty()
			.Must(path => path!.StartsWith('/'))
			.WithMessage("'{PropertyName}' must start with '/', for example '/_health'.");
		// Straight on RuleForEach, not inside ChildRules: FluentValidation does not run a child validator on a
		// null element, so a null entry passes startup and then throws out of the middleware.
		RuleForEach(x => x.RestrictedIPs)
			.Must(entry => IPEntry.TryParse(entry, out _))
			.WithMessage(
				"'{PropertyValue}' is not a valid entry. Use a single address such as 192.168.1.1, or CIDR " +
				"notation such as 192.168.1.0/24, where the number is a prefix length and the address is the start " +
				"of the block. Write each part in plain decimal with no leading zeros. Address ranges " +
				"(1.2.3.4-1.2.3.9) are no longer supported."
			);
	}
}
