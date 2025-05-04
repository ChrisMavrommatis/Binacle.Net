using System.Threading.RateLimiting;
using Binacle.Net.ServiceModule.Configuration;
using Binacle.Net.ServiceModule.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Binacle.Net.ServiceModule.Services;

internal class ApiUsageRateLimitingPolicy : IRateLimiterPolicy<string>
{
	private readonly IOptions<RateLimiterConfigurationOptions> options;
	private readonly ILogger<ApiUsageRateLimitingPolicy> logger;
	private readonly Func<OnRejectedContext, CancellationToken, ValueTask>?  onRejected;

	public ApiUsageRateLimitingPolicy(
		IOptions<RateLimiterConfigurationOptions> options,
		ILogger<ApiUsageRateLimitingPolicy> logger
	)
	{
		this.options = options;
		this.logger = logger;
		this.onRejected = (ctx, token) =>
		{
			logger.LogWarning("Request rejected by {Policy}", nameof(ApiUsageRateLimitingPolicy));
			return ValueTask.CompletedTask;
		};
	}
	
	public RateLimitPartition<string> GetPartition(HttpContext httpContext)
	{
		var user = httpContext.User;
		if (user?.Identity?.IsAuthenticated ?? false)
		{
			return RateLimitPartition.GetNoLimiter("Authenticated");
		}

		var configuration = RateLimiterConfigurationParser.Parse(this.options.Value.ApiUsageAnonymous);

		return RateLimiterConfigurationBuilder.Build(configuration, "ApiUsageAnonymous");
	}

	public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected => this.onRejected;
}
