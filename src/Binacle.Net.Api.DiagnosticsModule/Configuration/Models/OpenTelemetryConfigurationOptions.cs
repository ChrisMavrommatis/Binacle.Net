using Binacle.Net.Api.Kernel.Configuration.Models;

namespace Binacle.Net.Api.DiagnosticsModule.Configuration.Models;

internal class OpenTelemetryConfigurationOptions : IConfigurationOptions, IOpenTelemetryAttributes
{
	public static string FilePath => "DiagnosticsModule/OpenTelemetry.json";
	public static string SectionName => "OpenTelemetry";
	public static bool Optional => false;
	public static bool ReloadOnChange => true;
	public static string GetEnvironmentFilePath(string environment) => $"DiagnosticsModule/OpenTelemetry.{environment}.json";
	
	public string? ServiceNamespace { get; set; }
	public string? ServiceInstanceId { get; set; }
	public OtlpExporterConfigurationOptions Otlp { get; set; } = new();
	public AzureMonitorConfigurationOptions AzureMonitor { get; set; } = new();
	public Dictionary<string, object>? AdditionalAttributes { get; set; }

	public OpenTelemetryTracingConfigurationOptions Tracing { get; set; } = new();
	public OpenTelemetryMetricsConfigurationOptions Metrics { get; set; } = new();
	public OpenTelemetryLoggingConfigurationOptions Logging { get; set; } = new();

	public bool IsEnabled()
	{
		return this.Otlp.Enabled
		       || this.AzureMonitor.Enabled;
	}
}


