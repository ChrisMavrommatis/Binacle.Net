namespace Binacle.Net.Api.DiagnosticsModule.Configuration.Models;

internal interface IOpenTelemetryTypeConfigurationOptions
{
	bool Enabled { get; set; }
	OtlpExporterConfigurationOptions? Otlp { get; set; }
}

