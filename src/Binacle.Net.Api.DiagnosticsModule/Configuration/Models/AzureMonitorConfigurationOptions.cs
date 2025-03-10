﻿namespace Binacle.Net.Api.DiagnosticsModule.Configuration.Models;

internal class AzureMonitorConfigurationOptions
{
	public string? ConnectionString { get; set; }
	public bool EnableLiveMetrics { get; set; }
	public float SamplingRatio { get; set; }
}
