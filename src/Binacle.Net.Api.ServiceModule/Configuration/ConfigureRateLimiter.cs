using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Threading.RateLimiting;

namespace Binacle.Net.Api.ServiceModule.Configuration;

internal class ConfigureRateLimiter : IConfigureNamedOptions<RateLimiterOptions>
{
	private static string[] excludedPaths = [
		"/api/v1/query", 
		"/api/v2/query", 
		"/api/v2/pack"
	];

	public void Configure(string? name, RateLimiterOptions options)
	{
		this.ConfigureRateLimiterOptions(options);
	}

	public void Configure(RateLimiterOptions options)
	{
		this.ConfigureRateLimiterOptions(options);
	}

	private void ConfigureRateLimiterOptions(RateLimiterOptions options)
	{
		// TODO: Fix rate limiter as it breaks UI Module
		options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
		{
			if (!excludedPaths.Any(url => httpContext.Request.Path.StartsWithSegments(url)))
			{
				return RateLimitPartition.GetNoLimiter("NoLimiter");
			}
			
			var user = httpContext.User;
			if (user?.Identity?.IsAuthenticated ?? false)
			{
				return RateLimitPartition.GetNoLimiter("Authenticated");
			}

			return RateLimitPartition.GetFixedWindowLimiter("Anonymous", _ =>
			new FixedWindowRateLimiterOptions
			{
				Window = TimeSpan.FromSeconds(60),
				PermitLimit = 10,
				QueueLimit = 0,
				QueueProcessingOrder = QueueProcessingOrder.NewestFirst
			});
		});

		options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

	}
}

