using Binacle.Net.Kernel.Configuration.Models;
using Binacle.Net.Kernel.Network;
using FluentValidation;

namespace Binacle.Net.Configuration;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class ForwardedHeadersConfigurationOptions : IConfigurationOptions
{
	public static string FilePath => "ForwardedHeaders.json";
	public static string SectionName => "ForwardedHeaders";
	public static bool Optional => true;
	public static bool ReloadOnChange => true;
	public static string? GetEnvironmentFilePath(string environment) => $"ForwardedHeaders.{environment}.json";

	public bool Enabled { get; set; }

	public bool TrustLoopback { get; set; } = true;

	public bool TrustPrivateNetworks { get; set; } = true;

	public string[]? TrustedProxies { get; set; }

	public int ForwardLimit { get; set; } = 1;

	public string? ForwardedForHeaderName { get; set; }

	public bool HasNoTrustedSource()
	{
		return !this.TrustLoopback
		       && !this.TrustPrivateNetworks
		       && (this.TrustedProxies is null || this.TrustedProxies.Length == 0);
	}
}

internal class ForwardedHeadersConfigurationOptionsValidator : AbstractValidator<ForwardedHeadersConfigurationOptions>
{
	public ForwardedHeadersConfigurationOptionsValidator()
	{
		When(x => x.Enabled, () =>
		{
			// With nothing trusted the middleware stops checking rather than matching nothing, and every
			// caller's header is believed. Refuse to start instead of serving traffic on a forgeable address.
			RuleFor(x => x)
				.Must(options => !options.HasNoTrustedSource())
				.WithName(ForwardedHeadersConfigurationOptions.SectionName)
				.WithMessage(
					"Nothing is trusted to forward the caller's address. Set TrustLoopback or TrustPrivateNetworks, "
					+ "or add an entry to TrustedProxies."
				);

			RuleForEach(x => x.TrustedProxies)
				.Must(proxy => IPEntry.TryParse(proxy, out _))
				.WithMessage(
					"'{PropertyValue}' is not a valid entry. Use a CIDR range such as 172.16.0.0/12, or a single "
					+ "address such as 172.17.0.1. Write each part in plain decimal with no leading zeros. Address "
					+ "ranges (1.2.3.4-1.2.3.9) are not supported."
				);

			RuleFor(x => x.ForwardLimit)
				.GreaterThan(0)
				.WithMessage("'{PropertyName}' must be at least 1. It is how many proxies stand in front of the app.");
		});
	}
}
