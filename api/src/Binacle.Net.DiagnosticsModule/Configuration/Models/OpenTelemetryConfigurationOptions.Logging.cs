namespace Binacle.Net.DiagnosticsModule.Configuration.Models;

internal class OpenTelemetryLoggingConfigurationOptions : IOpenTelemetryAttributes
{
	public Dictionary<string, object>? AdditionalAttributes { get; set; }
}
