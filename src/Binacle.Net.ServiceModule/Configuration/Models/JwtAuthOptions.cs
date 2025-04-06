using Binacle.Net.Kernel.Configuration.Models;

namespace Binacle.Net.ServiceModule.Configuration.Models;

internal class JwtAuthOptions : IConfigurationOptions
{
	public static string FilePath => "ServiceModule/JwtAuth.json";
	public static string SectionName => "JwtAuth";
	public static bool Optional => false;
	public static bool ReloadOnChange => false;
	public static string GetEnvironmentFilePath(string environment) => $"ServiceModule/JwtAuth.{environment}.json";

	public string? Issuer { get; set; }
	public string? Audience { get; set; }
	public string? TokenSecret { get; set; }

	public int ExpirationInSeconds { get; set; }
}
