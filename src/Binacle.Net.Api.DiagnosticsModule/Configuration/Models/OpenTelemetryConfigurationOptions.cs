using Binacle.Net.Api.Kernel.Models;

namespace Binacle.Net.Api.DiagnosticsModule.Configuration.Models;

internal class OpenTelemetryConfigurationOptions : IConfigurationOptions
{
	public static string FilePath => "DiagnosticsModule/OpenTelemetry.json";
	public static string SectionName => "OpenTelemetry";
	public static bool Optional => false;
	public static bool ReloadOnChange => true;
	public static string GetEnvironmentFilePath(string environment) => $"DiagnosticsModule/OpenTelemetry.{environment}.json";
	
	public bool Enabled { get; set; }
	
	public string? ServiceNamespace { get; set; }
	public string? ServiceInstanceId { get; set; }
}
