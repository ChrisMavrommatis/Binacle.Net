namespace Binacle.Net.Api.DiagnosticsModule.Configuration.Models;

internal class OpenTelemetryMetricsConfigurationOptions : IOpenTelemetryAttributes
{
	public string[]? AdditionalMeters { get; set; }
	public Dictionary<string, object>? AdditionalAttributes { get; set; }
}

