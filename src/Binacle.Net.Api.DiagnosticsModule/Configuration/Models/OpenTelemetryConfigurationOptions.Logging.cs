namespace Binacle.Net.Api.DiagnosticsModule.Configuration.Models;

internal class OpenTelemetryLoggingConfiguration : IOpenTelemetryConfiguration
{
	public string? OtlpEndpoint { get; set; }

	public bool IsEnabled()
	{
		return !string.IsNullOrEmpty(this.OtlpEndpoint);
	}
}
