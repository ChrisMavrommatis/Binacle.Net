using Binacle.Net.Api.ServiceModule.Configuration.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using System.Threading.RateLimiting;

namespace Binacle.Net.Api.ServiceModule.Configuration;

internal class ConfigureRateLimiter : IConfigureNamedOptions<RateLimiterOptions>
{
	private readonly IOptions<RateLimiterConfigurationOptions> configOptions;

	public ConfigureRateLimiter(
		IOptions<RateLimiterConfigurationOptions> configOptions
	)
	{
		this.configOptions = configOptions;
	}

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
		options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

		if (!this.configOptions.Value.Enabled)
		{
			return;
		}

		options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
		{
			if (!ModuleConstants.RateLimitedPaths.Any(url => httpContext.Request.Path.StartsWithSegments(url)))
			{
				return RateLimitPartition.GetNoLimiter("NoLimiter");
			}

			var user = httpContext.User;
			if (user?.Identity?.IsAuthenticated ?? false)
			{
				return RateLimitPartition.GetNoLimiter("Authenticated");
			}

			var configuration = this.configOptions.Value.GetConfiguration();

			return configuration.Type switch
			{
				RateLimiterType.FixedWindow => RateLimitPartition.GetFixedWindowLimiter("Anonymous", _ =>
					new FixedWindowRateLimiterOptions
					{
						Window = TimeSpan.FromSeconds(configuration.WindowInSeconds),
						PermitLimit = configuration.PermitLimit,
						QueueLimit = 0,
						QueueProcessingOrder = QueueProcessingOrder.NewestFirst,
						AutoReplenishment = true,
					}
				),
				RateLimiterType.SlidingWindow => RateLimitPartition.GetSlidingWindowLimiter("Anonymous", _ =>
					new SlidingWindowRateLimiterOptions
					{
						Window = TimeSpan.FromSeconds(configuration.WindowInSeconds),
						PermitLimit = configuration.PermitLimit,
						QueueLimit = 0,
						QueueProcessingOrder = QueueProcessingOrder.NewestFirst,
						SegmentsPerWindow = configuration.SegmentsPerWindow!.Value,
						AutoReplenishment = true,
					}
				),
				_ => throw new NotImplementedException($"No Implementation found for RateLimiter of type {configuration.Type}")
			};

		});


	}
}

