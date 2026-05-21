using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Sockets;
using Binacle.Net.DiagnosticsModule.Configuration.Models;
using Binacle.Net.DiagnosticsModule.Models;

namespace Binacle.Net.DiagnosticsModule.Middleware;

internal class HealthChecksProtectionMiddleware
{
	private readonly RequestDelegate next;
	private readonly ILogger<HealthChecksProtectionMiddleware> logger;
	private readonly IOptions<HealthCheckConfigurationOptions> options;
	private readonly IPAddressRange[] restrictedIPAddressRanges;

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
			this.restrictedIPAddressRanges = options.Value.RestrictedIPs!.Select(IPAddressRange.ParseRange).ToArray();
		}
		else
		{
			this.restrictedIPAddressRanges = Array.Empty<IPAddressRange>();
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

		if (!this.restrictedIPAddressRanges.Any(range => range.IsInRange(remoteIp)))
		{
			logger.LogWarning("Health check request from {remoteIp} is not allowed", remoteIp);
			context.Response.StatusCode = StatusCodes.Status403Forbidden;
			return;
		}

		await next(context);
	}

}
