using Binacle.Net.Api.Kernel.Models;

namespace Binacle.Net.Api.DiagnosticsModule.Configuration.Models;

internal class HealthCheckConfigurationOptions : IConfigurationOptions
{
	public static string FilePath => "DiagnosticsModule/HealthChecks.json";
	public static string SectionName => "HealthChecks";
	public static bool Optional => false;
	public static bool ReloadOnChange => true;
	public static string GetEnvironmentFilePath(string environment) => $"DiagnosticsModule/HealthChecks.{environment}.json";

	public bool Enabled { get; set; }
	public string? Path { get; set; }
	public string[]? RestrictedIPs { get; set; }
	public string[]? RestrictedChecks { get; set; }
}

