// Microsoft.AspNetCore.HttpOverrides ships its own deprecated IPNetwork; the alias keeps this file on the
// System.Net one that KnownIPNetworks takes.
using IPNetwork = System.Net.IPNetwork;
using System.Net;
using Binacle.Net.Configuration;
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
			if (configuredOptions?.Enabled != true)
			{
				// Written rather than left alone. ASPNETCORE_FORWARDEDHEADERS_ENABLED switches the middleware on from
				// the environment with both trust lists emptied, which believes any caller's header. This delegate
				// runs after that one, so disabled stays disabled whatever the environment says.
				options.ForwardedHeaders = ForwardedHeaders.None;
				Log.Information("Forwarded headers. Status {status}", "Disabled");
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
				// A single address is just a range of one, but the middleware keeps them in separate lists.
				if (IPNetwork.TryParse(trustedProxy, out var trustedNetwork))
				{
					options.KnownIPNetworks.Add(trustedNetwork);
					continue;
				}

				options.KnownProxies.Add(IPAddress.Parse(trustedProxy));
			}

			var trustedSources = options.KnownIPNetworks
				.Select(network => network.ToString())
				.Concat(options.KnownProxies.Select(proxy => proxy.ToString()));

			Log.Information(
				"Forwarded headers. Status {status}. Trusting {trusted}. {forwardLimit} hop(s). Header {header}",
				"Enabled",
				string.Join(", ", trustedSources),
				configuredOptions.ForwardLimit,
				options.ForwardedForHeaderName
			);
		});

		return builder;
	}
}
