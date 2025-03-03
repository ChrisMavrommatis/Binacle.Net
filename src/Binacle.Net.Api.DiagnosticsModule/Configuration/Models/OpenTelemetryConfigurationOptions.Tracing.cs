using Binacle.Net.Api.Kernel.Models;

namespace Binacle.Net.Api.DiagnosticsModule.Configuration.Models;

internal class OpenTelemetryTracingConfiguration : IOpenTelemetryConfiguration
{
	public string? OtlpEndpoint { get; set; }
	
	public bool IsEnabled()
	{
		return !string.IsNullOrEmpty(this.OtlpEndpoint);
	}
}

