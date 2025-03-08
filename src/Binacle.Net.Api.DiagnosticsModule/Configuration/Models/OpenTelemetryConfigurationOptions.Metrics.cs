namespace Binacle.Net.Api.DiagnosticsModule.Configuration.Models;

internal class OpenTelemetryMetricsConfigurationOptions : IOpenTelemetryTypeConfigurationOptions
{
	public bool Enabled { get; set; }
	public OtlpExporterConfigurationOptions? Otlp { get; set; }
	public string[]? AdditionalMeters { get; set; }
}

