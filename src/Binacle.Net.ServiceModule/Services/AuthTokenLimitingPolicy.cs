using System.Threading.RateLimiting;
using Binacle.Net.ServiceModule.Configuration;
using Binacle.Net.ServiceModule.ExtensionMethods;
using Binacle.Net.ServiceModule.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Binacle.Net.ServiceModule.Services;

internal class AuthRateLimitingPolicy : IRateLimiterPolicy<string>
{
	private readonly IOptions<RateLimiterConfigurationOptions> options;
	private readonly ILogger<AuthRateLimitingPolicy> logger;
	private readonly Func<OnRejectedContext, CancellationToken, ValueTask>?  onRejected;

	public AuthRateLimitingPolicy(
		IOptions<RateLimiterConfigurationOptions> options,
		ILogger<AuthRateLimitingPolicy> logger
	)
	{
		this.options = options;
		this.logger = logger;
		this.onRejected = (ctx, token) =>
		{
			logger.LogWarning("Request rejected by {Policy}", nameof(AuthRateLimitingPolicy));
			return ValueTask.CompletedTask;
		};
	}
	
	public RateLimitPartition<string> GetPartition(HttpContext httpContext)
	{
		var partitionKey = httpContext?.GetClientIp() ?? "unknown";
		var configuration = RateLimiterConfigurationParser.Parse(this.options.Value.Auth);

		return RateLimiterConfigurationBuilder.Build(configuration, partitionKey);
	}

	public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected => this.onRejected;
}
