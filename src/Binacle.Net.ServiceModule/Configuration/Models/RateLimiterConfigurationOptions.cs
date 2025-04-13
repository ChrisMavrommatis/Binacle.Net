using Binacle.Net.Kernel.Configuration.Models;

namespace Binacle.Net.ServiceModule.Configuration.Models;

internal class RateLimiterConfigurationOptions : IConfigurationOptions
{
	public static string FilePath => "ServiceModule/RateLimiter.json";
	public static string SectionName => "RateLimiter";
	public static bool Optional => false;
	public static bool ReloadOnChange => false;
	public static string GetEnvironmentFilePath(string environment) => $"ServiceModule/RateLimiter.{environment}.json";

	public string? Anonymous { get; set; }
	public string? Auth { get; set; }
}
