using Binacle.Net.Api.Kernel.Models;

namespace Binacle.Net.Api.DiagnosticsModule.Configuration.Models;

internal class OpenTelemetryTracingConfigurationOptions : IOpenTelemetryTypeConfigurationOptions
{
	public bool Enabled { get; set; }
	public OtlpExporterConfigurationOptions? Otlp { get; set; }
	public string[]? AdditionalSources { get; set; }

}
