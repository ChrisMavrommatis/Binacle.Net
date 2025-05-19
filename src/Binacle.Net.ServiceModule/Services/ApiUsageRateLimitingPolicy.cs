using System.Threading.RateLimiting;
using Binacle.Net.ServiceModule.Configuration;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Models;
using Binacle.Net.ServiceModule.Models;
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
		// TODO: Review json config for default policies
		var user = httpContext.User;
		
		var anonymousRateLimiter = RateLimiterConfiguration.Get("ApiUsageAnonymous", this.options.Value.ApiUsageAnonymousConfiguration); 
		if (!(user?.Identity?.IsAuthenticated ?? false))
		{
			return anonymousRateLimiter;
		}
		
		var subscription = user.FindFirst(ApplicationClaimTypes.Subscription);
		var subscriptionType = user.FindFirst(ApplicationClaimTypes.SubscriptionType);
		
		if (subscription is null || subscriptionType is null)
		{
			return anonymousRateLimiter;
		}

		if (subscriptionType.Value == nameof(SubscriptionType.Demo))
		{
			return RateLimiterConfiguration.Get(subscription.Value, this.options.Value.ApiUsageDemoSubscriptionConfiguration);
		}
		
		return RateLimitPartition.GetNoLimiter("NormalSubscriptionNoLimiter");
	}

	public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected => this.onRejected;
}
