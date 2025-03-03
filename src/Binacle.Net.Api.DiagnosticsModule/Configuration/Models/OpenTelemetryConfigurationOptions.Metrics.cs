namespace Binacle.Net.Api.DiagnosticsModule.Configuration.Models;

internal class OpenTelemetryMetricsConfiguration : IOpenTelemetryConfiguration
{
	public string? OtlpEndpoint { get; set; }
	
	public bool IsEnabled()
	{
		return !string.IsNullOrEmpty(this.OtlpEndpoint);
	}
}

