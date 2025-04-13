using System.Threading.RateLimiting;
using Binacle.Net.ServiceModule.Models;

namespace Binacle.Net.ServiceModule.Helpers;

internal static class RateLimiterConfigurationBuilder
{
	public static RateLimitPartition<TKey> Build<TKey>(RateLimiterConfiguration configuration, TKey key)
	{
		return configuration.Type switch
		{
			RateLimiterType.FixedWindow => RateLimitPartition.GetFixedWindowLimiter(key, _ =>
				new FixedWindowRateLimiterOptions
				{
					Window = TimeSpan.FromSeconds(configuration.WindowInSeconds),
					PermitLimit = configuration.PermitLimit,
					QueueLimit = 0,
					QueueProcessingOrder = QueueProcessingOrder.NewestFirst,
					AutoReplenishment = true,
				}
			),
			RateLimiterType.SlidingWindow => RateLimitPartition.GetSlidingWindowLimiter(key, _ =>
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
			RateLimiterType.NoLimiter => RateLimitPartition.GetNoLimiter(key),
			_ => throw new NotImplementedException($"No Implementation found for RateLimiter of type {configuration.Type}")
		};
	} 
}
