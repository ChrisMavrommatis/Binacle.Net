using System.Threading.RateLimiting;

namespace Binacle.Net.ServiceModule.Models;

internal class RateLimiterConfiguration
{
	public RateLimiterType Type { get; set; }
	public int WindowInSeconds { get; set; }
	public int PermitLimit { get; set; }

	public int? SegmentsPerWindow { get; set; }
	
	public static RateLimitPartition<TKey> Get<TKey>(TKey partitionKey, RateLimiterConfiguration configuration)
	{
		return configuration.Type switch
		{
			RateLimiterType.FixedWindow => RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ =>
				new FixedWindowRateLimiterOptions
				{
					Window = TimeSpan.FromSeconds(configuration.WindowInSeconds),
					PermitLimit = configuration.PermitLimit,
					QueueLimit = 0,
					QueueProcessingOrder = QueueProcessingOrder.NewestFirst,
					AutoReplenishment = true,
				}
			),
			RateLimiterType.SlidingWindow => RateLimitPartition.GetSlidingWindowLimiter(partitionKey, _ =>
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
			RateLimiterType.NoLimiter => RateLimitPartition.GetNoLimiter(partitionKey),
			_ => throw new NotImplementedException($"No Implementation found for RateLimiter of type {configuration.Type}")
		};
	} 
	public static bool TryParse(string? value, out RateLimiterConfiguration? configuration)
	{
		try
		{
			configuration = Parse(value);
			return true;
		}
		catch (Exception)
		{
			configuration = null;
			return false;
		}
	}
	
	public static RateLimiterConfiguration Parse(string? value)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			throw new InvalidOperationException("RateLimiter configuration is not set");
		}

		var parts = value.Split("::");
		if (parts.Length != 2)
		{
			throw new InvalidOperationException("RateLimiter configuration must be in the format of 'Type::Options'");

		}

		var type = parts[0];
		var options = parts[1];

		return type switch
		{
			nameof(RateLimiterType.FixedWindow) => CreateFixedWindow(options),
			nameof(RateLimiterType.SlidingWindow) => CreateSlidingWindow(options),
			nameof(RateLimiterType.NoLimiter) => CreateNoLimiter(options),
			_ => throw new InvalidOperationException("RateLimiter type must be of 'FixedWindow' or 'SlidingWindow' ")
		};
	}

	private static RateLimiterConfiguration CreateFixedWindow(string options)
	{
		var parts = options.Split('/');

		if (parts.Length != 2)
		{
			throw new InvalidOperationException("FixedWindow options must be in the format of 'PermitLimit/WindowInSeconds'");
		}

		return new RateLimiterConfiguration
		{
			Type = RateLimiterType.FixedWindow,
			PermitLimit = int.Parse(parts[0]),
			WindowInSeconds = int.Parse(parts[1])
		};

	}

	private static RateLimiterConfiguration CreateSlidingWindow(string options)
	{
		var parts = options.Split('/');
		if(parts.Length != 2)
		{
			throw new InvalidOperationException("SlidingWindow options must be in the format of 'PermitLimit/WindowInSeconds-SegmentsPerWindow'");
		}

		var parts2 = parts[1].Split('-');
		if (parts2.Length != 2)
		{
			throw new InvalidOperationException("SlidingWindow options must be in the format of 'PermitLimit/WindowInSeconds-SegmentsPerWindow'");
		}

		return new RateLimiterConfiguration
		{
			Type = RateLimiterType.SlidingWindow,
			PermitLimit = int.Parse(parts[0]),
			WindowInSeconds = int.Parse(parts2[0]),
			SegmentsPerWindow = int.Parse(parts2[1])
		};
	}
	
	private static RateLimiterConfiguration CreateNoLimiter(string options)
	{
		return new RateLimiterConfiguration
		{
			Type = RateLimiterType.NoLimiter,
			PermitLimit = 0,
			WindowInSeconds = 0
		};
	}
}

internal enum RateLimiterType
{
	NoLimiter,
	FixedWindow,
	SlidingWindow
}
