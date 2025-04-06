namespace Binacle.Net.ServiceModule.Configuration.Models;

internal class RateLimiterConfiguration
{
	public RateLimiterType Type { get; set; }
	public int WindowInSeconds { get; set; }
	public int PermitLimit { get; set; }

	public int? SegmentsPerWindow { get; set; }
}
