namespace Binacle.Net.DiagnosticsModule.Configuration.Models;

internal interface IOpenTelemetryAttributes
{
	Dictionary<string, object>? AdditionalAttributes { get; set; }
}
