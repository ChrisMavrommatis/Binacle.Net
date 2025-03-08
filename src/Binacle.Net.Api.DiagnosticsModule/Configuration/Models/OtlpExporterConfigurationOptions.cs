namespace Binacle.Net.Api.DiagnosticsModule.Configuration.Models;

internal class OtlpExporterConfigurationOptions
{
	public string? Endpoint { get; set; }
	public string? Protocol { get; set; }
	public string? Headers { get; set; }
}
