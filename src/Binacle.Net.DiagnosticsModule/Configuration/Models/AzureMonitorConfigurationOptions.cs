namespace Binacle.Net.DiagnosticsModule.Configuration.Models;

internal class AzureMonitorConfigurationOptions
{
	public bool Enabled { get; set; }
	public string? ConnectionString { get; set; }
	public bool EnableLiveMetrics { get; set; }
	public float SamplingRatio { get; set; }
}
