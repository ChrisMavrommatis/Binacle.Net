using System.Threading.RateLimiting;
using Binacle.Net.ServiceModule.Configuration;
using Binacle.Net.ServiceModule.ExtensionMethods;
using Binacle.Net.ServiceModule.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Binacle.Net.ServiceModule.Services;

internal class AuthTokenRateLimitingPolicy : IRateLimiterPolicy<string>
{
	private readonly IOptions<RateLimiterConfigurationOptions> options;
	private readonly ILogger<AuthTokenRateLimitingPolicy> logger;
	private readonly Func<OnRejectedContext, CancellationToken, ValueTask>?  onRejected;

	public AuthTokenRateLimitingPolicy(
		IOptions<RateLimiterConfigurationOptions> options,
		ILogger<AuthTokenRateLimitingPolicy> logger
	)
	{
		this.options = options;
		this.logger = logger;
		this.onRejected = (ctx, token) =>
		{
			logger.LogWarning("Request rejected by {Policy}", nameof(AuthTokenRateLimitingPolicy));
			return ValueTask.CompletedTask;
		};
	}
	
	public RateLimitPartition<string> GetPartition(HttpContext httpContext)
	{
		var partitionKey = httpContext?.GetClientIp() ?? "unknown";
		return RateLimiterConfiguration.Get(partitionKey, this.options.Value.AuthTokenConfiguration);
	}
	
	public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected => this.onRejected;
}
