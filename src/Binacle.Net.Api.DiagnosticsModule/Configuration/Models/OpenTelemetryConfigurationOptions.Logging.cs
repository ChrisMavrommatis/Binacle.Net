namespace Binacle.Net.Api.DiagnosticsModule.Configuration.Models;

internal class OpenTelemetryLoggingConfigurationOptions : IOpenTelemetryAttributes
{
	public Dictionary<string, object>? AdditionalAttributes { get; set; }
}
