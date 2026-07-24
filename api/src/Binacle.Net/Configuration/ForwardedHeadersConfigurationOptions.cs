using System.Net;
using Binacle.Net.Kernel.Configuration.Models;
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

	// Off by default. With nothing in front of the app the connection address is already the caller's and cannot be
	// forged, so reading the headers would only replace it with a value the caller controls.
	public bool Enabled { get; set; }

	// The three trust settings below widen in order. Each one describes where the proxy sits, which is something an
	// operator knows without knowing anything about this middleware.

	// A proxy on the same machine. Nothing outside the host can present a loopback address — a spoofed source never
	// completes the handshake — so this costs nothing and is left on.
	public bool TrustLoopback { get; set; } = true;

	// A proxy on a container or local network, which is where it sits in almost every container deployment. Wider
	// than loopback: any host on those ranges that can open a connection is trusted to rewrite the caller.
	public bool TrustPrivateNetworks { get; set; } = true;

	// Anything else, named exactly. CIDR ranges or single addresses. Added to whatever the flags above allow, so an
	// environment override can extend the trust list without having to restate it.
	public string[]? TrustedProxies { get; set; }

	// How many proxies stand in front. Entries beyond this are left in the header and ignored, so padding the header
	// cannot push the result further back than the real topology.
	public int ForwardLimit { get; set; } = 1;

	// Most CDNs also send a single-value header they overwrite on every request, leaving nothing of what the caller
	// sent. Name that header here to read it instead of X-Forwarded-For.
	public string? ForwardedForHeaderName { get; set; }

	public bool HasNoTrustedSource() =>
		!this.TrustLoopback
		&& !this.TrustPrivateNetworks
		&& (this.TrustedProxies is null || this.TrustedProxies.Length == 0);
}

internal class ForwardedHeadersConfigurationOptionsValidator : AbstractValidator<ForwardedHeadersConfigurationOptions>
{
	public ForwardedHeadersConfigurationOptionsValidator()
	{
		When(x => x.Enabled, () =>
		{
			// With nothing trusted the middleware stops checking altogether rather than matching nothing, and every
			// caller's header is believed. Refuse to start instead of serving traffic on a forgeable address.
			RuleFor(x => x)
				.Must(options => !options.HasNoTrustedSource())
				.WithMessage(
					"Nothing is trusted to forward the caller's address. Set TrustLoopback or TrustPrivateNetworks, "
					+ "or add an entry to TrustedProxies."
				);

			RuleForEach(x => x.TrustedProxies)
				.Must(proxy => IPNetwork.TryParse(proxy, out _) || IPAddress.TryParse(proxy, out _))
				.WithMessage(
					"Invalid entry. Use a CIDR range such as 172.16.0.0/12, or a single address such as 172.17.0.1"
				);

			RuleFor(x => x.ForwardLimit)
				.GreaterThan(0)
				.WithMessage("ForwardLimit must be at least 1");
		});
	}
}
