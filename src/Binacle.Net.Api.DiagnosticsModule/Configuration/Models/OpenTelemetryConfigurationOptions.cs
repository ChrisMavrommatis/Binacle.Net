using Binacle.Net.Api.Kernel.Models;

namespace Binacle.Net.Api.DiagnosticsModule.Configuration.Models;

internal class OpenTelemetryConfigurationOptions : IConfigurationOptions
{
	public static string FilePath => "DiagnosticsModule/OpenTelemetry.json";
	public static string SectionName => "OpenTelemetry";
	public static bool Optional => false;
	public static bool ReloadOnChange => true;
	public static string GetEnvironmentFilePath(string environment) => $"DiagnosticsModule/OpenTelemetry.{environment}.json";
	
	public string? ServiceNamespace { get; set; }
	public string? ServiceInstanceId { get; set; }
	
	public string? GlobalOtlpEndpoint { get; set; }
	public OpenTelemetryTracingConfiguration Tracing { get; set; } = new();
	public OpenTelemetryMetricsConfiguration Metrics { get; set; } = new();
	public OpenTelemetryLoggingConfiguration Logging { get; set; } = new();
	

	public bool IsEnabled()
	{
		if (!string.IsNullOrEmpty(this.GlobalOtlpEndpoint))
			return true;
		
		return (this.Tracing?.IsEnabled() ?? false)
			|| (this.Metrics?.IsEnabled() ?? false)
			|| (this.Logging?.IsEnabled() ?? false);

	}
	
	public bool IsEnabled<TConfiguration>(Func<OpenTelemetryConfigurationOptions, TConfiguration> configurationSelector)
		where TConfiguration : IOpenTelemetryConfiguration
	{
		if(!string.IsNullOrEmpty(this.GlobalOtlpEndpoint))
			return true;
		
		var configuration = configurationSelector(this);
		return configuration.IsEnabled();
	}
}


