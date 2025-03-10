using Binacle.Net.Api.Kernel.Models;

namespace Binacle.Net.Api.DiagnosticsModule.Configuration.Models;

internal class OpenTelemetryConfigurationOptions : IConfigurationOptions
{
	public static string FilePath => "DiagnosticsModule/OpenTelemetry.json";
	public static string SectionName => "OpenTelemetry";
	public static bool Optional => false;
	public static bool ReloadOnChange => true;
	public static string GetEnvironmentFilePath(string environment) => $"DiagnosticsModule/OpenTelemetry.{environment}.json";
	
	public string? ServiceNamespace { get; set; }
	public string? ServiceInstanceId { get; set; }
	
	public Dictionary<string, object>? AdditionalAttributes { get; set; }
	public OtlpExporterConfigurationOptions? Otlp { get; set; }
	public AzureMonitorConfigurationOptions? AzureMonitor { get; set; }
	public OpenTelemetryTracingConfigurationOptions Tracing { get; set; } = new();
	public OpenTelemetryMetricsConfigurationOptions Metrics { get; set; } = new();
	public OpenTelemetryLoggingConfigurationOptions Logging { get; set; } = new();
	

	public bool IsEnabled()
	{
		return this.Tracing.Enabled
			|| this.Metrics.Enabled
			|| this.Logging.Enabled;
	}

	public bool UseIndividualOtlpEndpointFor<TConfiguration>(
		Func<OpenTelemetryConfigurationOptions, TConfiguration> configurationSelector
	)
		where TConfiguration: IOpenTelemetryTypeConfigurationOptions
	{
		if (!string.IsNullOrEmpty(this.Otlp?.Endpoint))
		{
			return false;
		}

		var configuration = configurationSelector(this);
		return !string.IsNullOrEmpty(configuration.Otlp?.Endpoint);
	}
}


