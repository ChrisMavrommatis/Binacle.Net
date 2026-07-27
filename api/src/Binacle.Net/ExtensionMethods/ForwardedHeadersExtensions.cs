// Microsoft.AspNetCore.HttpOverrides ships its own deprecated IPNetwork; the alias keeps this file on the
// System.Net one that KnownIPNetworks takes.
using IPNetwork = System.Net.IPNetwork;
using Binacle.Net.Configuration;
using Binacle.Net.Kernel.Network;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;

namespace Binacle.Net.ExtensionMethods;

internal static class ForwardedHeadersExtensions
{
	private static readonly IPNetwork[] privateNetworks =
	[
		IPNetwork.Parse("10.0.0.0/8"),
		IPNetwork.Parse("172.16.0.0/12"),
		IPNetwork.Parse("192.168.0.0/16"),
		IPNetwork.Parse("fc00::/7")
	];

	public static WebApplicationBuilder ConfigureForwardedHeaders(this WebApplicationBuilder builder)
	{
		var configuredOptions = builder.Configuration
			.GetSection(ForwardedHeadersConfigurationOptions.SectionName)
			.Get<ForwardedHeadersConfigurationOptions>();

		builder.Services.Configure<ForwardedHeadersOptions>(options =>
		{
			Apply(configuredOptions, options);

			if (options.ForwardedHeaders == ForwardedHeaders.None)
			{
				Log.Information("Forwarded headers. Status {status}", "Disabled");
				return;
			}

			var trustedSources = options.KnownIPNetworks
				.Select(network => network.ToString())
				.Concat(options.KnownProxies.Select(proxy => proxy.ToString()));

			Log.Information(
				"Forwarded headers. Status {status}. Trusting {trusted}. {forwardLimit} hop(s). Header {header}",
				"Enabled",
				string.Join(", ", trustedSources),
				options.ForwardLimit,
				options.ForwardedForHeaderName
			);
		});

		return builder;
	}

	// The whole mapping from our configuration onto the framework's, and nothing else: no host, no container, no
	// logging. Everything the middleware goes on to do is decided in here, which is what makes it testable on its
	// own.
	internal static void Apply(
		ForwardedHeadersConfigurationOptions? configuredOptions,
		ForwardedHeadersOptions options
	)
	{
		if (configuredOptions?.Enabled != true)
		{
			// Written rather than left alone. ASPNETCORE_FORWARDEDHEADERS_ENABLED switches the middleware on from
			// the environment with both trust lists emptied, which believes any caller's header. This runs after
			// that one, so disabled stays disabled whatever the environment says.
			options.ForwardedHeaders = ForwardedHeaders.None;
			return;
		}

		options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
		options.ForwardLimit = configuredOptions.ForwardLimit;

		if (!string.IsNullOrWhiteSpace(configuredOptions.ForwardedForHeaderName))
		{
			options.ForwardedForHeaderName = configuredOptions.ForwardedForHeaderName;
		}

		// The framework seeds these with loopback. Clearing is only safe because the validator refuses to start
		// with nothing trusted — two empty lists switch the check off entirely rather than matching nothing.
		if (!configuredOptions.TrustLoopback)
		{
			options.KnownIPNetworks.Clear();
			options.KnownProxies.Clear();
		}

		if (configuredOptions.TrustPrivateNetworks)
		{
			foreach (var privateNetwork in privateNetworks)
			{
				options.KnownIPNetworks.Add(privateNetwork);
			}
		}

		foreach (var trustedProxy in configuredOptions.TrustedProxies ?? [])
		{
			// Startup validation rejects a malformed entry, so reaching this is a bug. It throws rather than
			// skipping the entry, because a dropped one silently narrows who is trusted and leaves the app
			// reading a header from a proxy it no longer recognises.
			if (!IPEntry.TryParse(trustedProxy, out var trustedNetwork))
			{
				throw new InvalidOperationException(
					$"Invalid TrustedProxies entry: '{trustedProxy}'. Use a single address such as 172.17.0.1, or " +
					"CIDR notation such as 172.16.0.0/12"
				);
			}

			// A single address parses to a network of one, so both forms go in the same list. The middleware
			// checks KnownProxies and KnownIPNetworks alike, so nothing is lost by not splitting them.
			options.KnownIPNetworks.Add(trustedNetwork);
		}
	}
}
