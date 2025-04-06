using Binacle.Net.Kernel.Configuration.Models;

namespace Binacle.Net.DiagnosticsModule.Configuration.Models;

internal class OpenTelemetryTracingConfigurationOptions : IOpenTelemetryAttributes
{
	public string[]? AdditionalSources { get; set; }
	public Dictionary<string, object>? AdditionalAttributes { get; set; }
}
