using Binacle.Net.Api.Kernel.Models;

namespace Binacle.Net.Api.ServiceModule.Configuration.Models;

internal class RateLimiterConfigurationOptions : IConfigurationOptions
{
	public static string FilePath => "ServiceModule/RateLimiter.json";
	public static string SectionName => "RateLimiter";
	public static bool Optional => false;
	public static bool ReloadOnChange => false;
	public static string GetEnvironmentFilePath(string environment) => $"ServiceModule/RateLimiter.{environment}.json";

	public string? Anonymous { get; set; }

	public static RateLimiterConfiguration ParseConfiguration(string? value)
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
			"FixedWindow" => ParseFixedWindowOptions(options),
			"SlidingWindow" => ParseSlidingWindowOptions(options),
			_ => throw new InvalidOperationException("RateLimiter type must be of 'FixedWindow' or 'SlidingWindow' ")
		};
	}

	private static RateLimiterConfiguration ParseFixedWindowOptions(string options)
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

	private static RateLimiterConfiguration ParseSlidingWindowOptions(string options)
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
	public RateLimiterConfiguration GetConfiguration() => ParseConfiguration(this.Anonymous);
}
