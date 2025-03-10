using OpenTelemetry.Exporter;

namespace Binacle.Net.Api.DiagnosticsModule.Configuration.Models;

internal class OtlpExporterConfigurationOptions
{
	public string? Endpoint { get; set; }
	public string? Protocol { get; set; }
	public string? Headers { get; set; }
	
	public OtlpExportProtocol? GetOtlpExportProtocol()
	{
		if(string.IsNullOrEmpty(this.Protocol))
		{
			return null;
		}
		return this.Protocol switch
		{
			"grpc" => OtlpExportProtocol.Grpc,
			"httpProtobuf" => OtlpExportProtocol.HttpProtobuf,
			_ => throw new ArgumentOutOfRangeException(nameof(this.Protocol), this.Protocol, null)
		};
	}
}
