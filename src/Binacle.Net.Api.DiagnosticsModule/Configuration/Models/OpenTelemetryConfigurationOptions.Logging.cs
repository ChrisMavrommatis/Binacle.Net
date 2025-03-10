namespace Binacle.Net.Api.DiagnosticsModule.Configuration.Models;

internal class OpenTelemetryLoggingConfigurationOptions : IOpenTelemetryTypeConfigurationOptions
{
	public bool Enabled { get; set; }
	public OtlpExporterConfigurationOptions? Otlp { get; set; }
	public Dictionary<string, object>? AdditionalAttributes { get; set; }
}
