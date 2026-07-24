using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using Binacle.Net.DiagnosticsModule.Configuration.Models;
using Binacle.Net.DiagnosticsModule.Models;

namespace Binacle.Net.DiagnosticsModule.Middleware;

internal class HealthChecksProtectionMiddleware
{
	private readonly RequestDelegate next;
	private readonly ILogger<HealthChecksProtectionMiddleware> logger;
	private readonly IOptions<HealthCheckConfigurationOptions> options;
	private readonly IPNetwork[] restrictedIPAddressRanges;

	public HealthChecksProtectionMiddleware(
		RequestDelegate next,
		ILogger<HealthChecksProtectionMiddleware> logger,
		IOptions<HealthCheckConfigurationOptions> options
	)
	{
		this.next = next;
		this.logger = logger;
		this.options = options;
		if (this.options.Value.RestrictedIPs is not null && this.options.Value.RestrictedIPs.Length > 0)
		{
			this.restrictedIPAddressRanges = options.Value.RestrictedIPs!
				.Select(restrictedIp =>
				{
					// Startup validation rejects a malformed entry, so reaching this is a bug. Throwing beats keeping
					// the default network: that one has no base address, so it would turn every health request into an
					// error, and a silently dropped entry would quietly widen or narrow the allow-list instead.
					if (!RestrictedIPNetwork.TryParse(restrictedIp, out var network))
					{
						throw new InvalidOperationException(
							$"Invalid health check RestrictedIPs entry: '{restrictedIp}'. Use a single address such as " +
							"192.168.1.1, or CIDR notation such as 192.168.1.0/24"
						);
					}

					return network;
				})
				.ToArray();
		}
		else
		{
			this.restrictedIPAddressRanges = Array.Empty<IPNetwork>();
		}
	}

	public async Task InvokeAsync(HttpContext context)
	{
		// Ignore if the request is not for the health checks path
		if (!context.Request.Path.StartsWithSegments(this.options.Value.Path))
		{
			await next(context);
			return;
		}

		if(this.restrictedIPAddressRanges.Length == 0)
		{
			await next(context);
			return;
		}

		// Check if the request is allowed based on ip

		var remoteIp = context.Connection.RemoteIpAddress;
		// is in range

		if (remoteIp is null) 
		{
			logger.LogWarning("Health check request from unknown remote IP");
			context.Response.StatusCode = StatusCodes.Status403Forbidden;
			return;
		}

		// Behind a proxy this is the caller only once the forwarded headers middleware has resolved it; without that
		// it is the proxy, and no operator address will ever match.
		var callerAddress = RestrictedIPNetwork.Normalize(remoteIp);

		if (!this.restrictedIPAddressRanges.Any(range => range.Contains(callerAddress)))
		{
			logger.LogWarning("Health check request from {remoteIp} is not allowed", remoteIp);
			context.Response.StatusCode = StatusCodes.Status403Forbidden;
			return;
		}

		await next(context);
	}

}
