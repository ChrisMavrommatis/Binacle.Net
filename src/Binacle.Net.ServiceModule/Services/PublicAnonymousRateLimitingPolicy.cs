using System.Threading.RateLimiting;
using Binacle.Net.ServiceModule.Configuration.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Binacle.Net.ServiceModule.Services;

internal class PublicAnonymousRateLimitingPolicy : IRateLimiterPolicy<string>
{
	private readonly IOptions<RateLimiterConfigurationOptions> options;
	private readonly ILogger<PublicAnonymousRateLimitingPolicy> logger;

	public PublicAnonymousRateLimitingPolicy(
		IOptions<RateLimiterConfigurationOptions> options,
		ILogger<PublicAnonymousRateLimitingPolicy> logger
	)
	{
		this.options = options;
		this.logger = logger;
	}
	
	public RateLimitPartition<string> GetPartition(HttpContext httpContext)
	{
		return RateLimitPartition.GetNoLimiter("public-anonymous");
	}

	public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected { get; }
}
