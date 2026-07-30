using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using Binacle.Net.DiagnosticsModule.Configuration.Models;
using Binacle.Net.Kernel.Network;

namespace Binacle.Net.DiagnosticsModule.Middleware;

internal class HealthChecksProtectionMiddleware
{
	private readonly RequestDelegate next;
	private readonly ILogger<HealthChecksProtectionMiddleware> logger;
	private readonly IOptions<HealthCheckConfigurationOptions> options;
	private readonly HealthCheckAllowList allowList;

	public HealthChecksProtectionMiddleware(
		RequestDelegate next,
		ILogger<HealthChecksProtectionMiddleware> logger,
		IOptions<HealthCheckConfigurationOptions> options
	)
	{
		this.next = next;
		this.logger = logger;
		this.options = options;
		// Built once. HealthChecks.json is not reloaded (see HealthCheckConfigurationOptions.ReloadOnChange), so
		// moving to IOptionsMonitor means calling this again on change and changing nothing else.
		this.allowList = this.BuildAllowList(options.Value.RestrictedIPs);
	}

	internal HealthCheckAllowList BuildAllowList(string?[]? entries)
	{
		if (entries is null || entries.Length == 0)
		{
			return new HealthCheckAllowList([]);
		}

		var networks = new IPNetwork[entries.Length];

		for (var index = 0; index < entries.Length; index++)
		{
			var entry = entries[index];

			// Startup validation rejects a malformed entry, so reaching this is a bug. Throwing beats keeping the
			// default network: that one has no base address, so it would turn every health request into an error,
			// and a silently dropped entry would quietly widen or narrow the allow-list instead. The position is in
			// the message because the entry itself can be null or blank.
			if (!IPEntry.TryParse(entry, out var network))
			{
				throw new InvalidOperationException(
					$"Invalid health check RestrictedIPs entry at position {index}: '{entry}'. Use a single address " +
					"such as 192.168.1.1, or CIDR notation such as 192.168.1.0/24"
				);
			}

			networks[index] = network;

			// "192.168.1.1/24" is the whole 192.168.1.0/24, which is 256 hosts an entry naming one does not look
			// like. Every parser in .NET masks the host bits off in silence; this one says what it ended up with.
			if (entry!.Contains('/') && entry.Trim() != network.ToString())
			{
				this.logger.LogWarning(
					"Health check RestrictedIPs entry {entry} covers the whole network {network}", entry, network
				);
			}
		}

		return new HealthCheckAllowList(networks);
	}

	public async Task InvokeAsync(HttpContext context)
	{
		// Ignore if the request is not for the health checks path
		if (!context.Request.Path.StartsWithSegments(this.options.Value.Path))
		{
			await next(context);
			return;
		}

		if (this.allowList.RestrictsNobody)
		{
			await next(context);
			return;
		}

		// Behind a proxy this is the caller only once the forwarded headers middleware has resolved it; without that
		// it is the proxy, and no operator address will ever match.
		var remoteIp = context.Connection.RemoteIpAddress;

		if (remoteIp is null)
		{
			logger.LogWarning("Health check request from unknown remote IP");
			context.Response.StatusCode = StatusCodes.Status403Forbidden;
			return;
		}

		if (!this.allowList.Allows(remoteIp))
		{
			logger.LogWarning("Health check request from {remoteIp} is not allowed", remoteIp);
			context.Response.StatusCode = StatusCodes.Status403Forbidden;
			return;
		}

		await next(context);
	}

	// The parsed RestrictedIPs, and the only thing that knows what they mean. Nested rather than file-scoped:
	// CS9051 forbids a file-local type in a member signature of a non-file-local type, which the field and
	// BuildAllowList both are. An allow-list anything else needed would belong in the Kernel beside IPEntry.
	internal sealed class HealthCheckAllowList
	{
		private readonly IPNetwork[] networks;

		public HealthCheckAllowList(IPNetwork[] networks)
		{
			this.networks = networks;
		}

		// An empty list is not "allow nobody" - it is the default deployment, where the health check is open.
		public bool RestrictsNobody => this.networks.Length == 0;

		public bool Allows(IPAddress caller)
		{
			var callerAddress = IPEntry.Normalize(caller);

			foreach (var network in this.networks)
			{
				if (network.Contains(callerAddress))
				{
					return true;
				}
			}

			return false;
		}
	}
}
