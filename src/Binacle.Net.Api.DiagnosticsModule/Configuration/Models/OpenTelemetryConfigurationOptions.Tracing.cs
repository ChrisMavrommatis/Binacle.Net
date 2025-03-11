using Binacle.Net.Api.Kernel.Models;

namespace Binacle.Net.Api.DiagnosticsModule.Configuration.Models;

internal class OpenTelemetryTracingConfigurationOptions : IOpenTelemetryAttributes
{
	public string[]? AdditionalSources { get; set; }
	public Dictionary<string, object>? AdditionalAttributes { get; set; }
}
