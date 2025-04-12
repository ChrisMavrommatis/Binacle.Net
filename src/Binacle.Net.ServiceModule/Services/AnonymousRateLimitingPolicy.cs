using System.Threading.RateLimiting;
using Binacle.Net.ServiceModule.Configuration.Models;
using Binacle.Net.ServiceModule.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Binacle.Net.ServiceModule.Services;

internal class AnonymousRateLimitingPolicy : IRateLimiterPolicy<string>
{
	private readonly IOptions<RateLimiterConfigurationOptions> options;
	private readonly ILogger<AnonymousRateLimitingPolicy> logger;
	private readonly Func<OnRejectedContext, CancellationToken, ValueTask>?  onRejected;

	public AnonymousRateLimitingPolicy(
		IOptions<RateLimiterConfigurationOptions> options,
		ILogger<AnonymousRateLimitingPolicy> logger
	)
	{
		this.options = options;
		this.logger = logger;
		this.onRejected = (ctx, token) =>
		{
			logger.LogWarning($"Request rejected by {nameof(AnonymousRateLimitingPolicy)}");
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

		var configuration = RateLimiterConfigurationParser.Parse(this.options.Value.Anonymous);

		return RateLimiterConfigurationBuilder.Build(configuration, "Anonymous");
	}

	public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected => this.onRejected;
}
