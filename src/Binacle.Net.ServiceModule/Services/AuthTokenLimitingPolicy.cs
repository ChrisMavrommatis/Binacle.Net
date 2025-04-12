using System.Threading.RateLimiting;
using Binacle.Net.ServiceModule.Configuration.Models;
using Binacle.Net.ServiceModule.ExtensionMethods;
using Binacle.Net.ServiceModule.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Binacle.Net.ServiceModule.Services;

internal class AuthTokenLimitingPolicy : IRateLimiterPolicy<string>
{
	private readonly IOptions<RateLimiterConfigurationOptions> options;
	private readonly ILogger<AuthTokenLimitingPolicy> logger;
	private readonly Func<OnRejectedContext, CancellationToken, ValueTask>?  onRejected;

	public AuthTokenLimitingPolicy(
		IOptions<RateLimiterConfigurationOptions> options,
		ILogger<AuthTokenLimitingPolicy> logger
	)
	{
		this.options = options;
		this.logger = logger;
		this.onRejected = (ctx, token) =>
		{
			logger.LogWarning($"Request rejected by {nameof(AuthTokenLimitingPolicy)}");
			return ValueTask.CompletedTask;
		};
	}
	
	public RateLimitPartition<string> GetPartition(HttpContext httpContext)
	{
		var partitionKey = httpContext.User.Identity?.Name ?? httpContext.GetClientIp();
		
		var configuration = RateLimiterConfigurationParser.Parse(this.options.Value.Anonymous);

		return RateLimiterConfigurationBuilder.Build(configuration, partitionKey ?? "unknown");
	}

	public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected => this.onRejected;
}
