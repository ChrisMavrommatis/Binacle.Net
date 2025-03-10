using Azure.Monitor.OpenTelemetry.AspNetCore;
using Binacle.Net.Api.DiagnosticsModule.Configuration.Models;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Instrumentation.Http;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Binacle.Net.Api.DiagnosticsModule.ExtensionMethods;

internal static class OpenTelemetryConfigurationExtensions
{
	public static MeterProviderBuilder AddMeters(
		this MeterProviderBuilder meterProviderBuilder,
		string[]? additionalMeters
	)
	{
		if (additionalMeters?.Any() ?? false)
		{
			foreach (var meter in additionalMeters)
			{
				meterProviderBuilder.AddMeter(meter);
			}
		}
		return meterProviderBuilder;
	}
	
	public static TracerProviderBuilder AddSources(
		this TracerProviderBuilder tracerProviderBuilder,
		string[]? additionalSources
	)
	{
		if (additionalSources?.Any() ?? false)
		{
			foreach (var source in additionalSources)
			{
				tracerProviderBuilder.AddSource(source);
			}
		}
		return tracerProviderBuilder;
	}

	public static void ConfigureOtlpExporter(
		this OtlpExporterOptions options,
		OtlpExporterConfigurationOptions configurationOptions
		)
	{
		options.Endpoint = new Uri(configurationOptions.Endpoint!);

		var protocol = configurationOptions.GetOtlpExportProtocol();
		if (protocol is not null)
		{
			options.Protocol = protocol.Value;
		}

		if (!string.IsNullOrEmpty(configurationOptions.Headers))
		{
			options.Headers = configurationOptions.Headers;
		}
	}

	public static void ConfigureAspNetCoreInstrumentation(
		this AspNetCoreTraceInstrumentationOptions options,
		OpenTelemetryTracingConfigurationOptions tracingConfigurationOptions
		)
	{
	}
	
	public static void ConfigureHttpClientInstrumentation(
		this HttpClientTraceInstrumentationOptions options,
		OpenTelemetryTracingConfigurationOptions tracingConfigurationOptions
	)
	{
	}
	
	public static void ConfigureAzureMonitor(
		this AzureMonitorOptions options,
		AzureMonitorConfigurationOptions azureMonitorConfigurationOptions
	)
	{
		options.ConnectionString = azureMonitorConfigurationOptions.ConnectionString!;
		options.SamplingRatio = azureMonitorConfigurationOptions.SamplingRatio;
		options.EnableLiveMetrics = azureMonitorConfigurationOptions.EnableLiveMetrics;
	}

	public static ResourceBuilder AddOptionalAdditionalAttributes(
		this ResourceBuilder builder,
		Dictionary<string, object>? additionalAttributes
		)
	{
		if (additionalAttributes?.Any() ?? false)
		{
			builder.AddAttributes(additionalAttributes!);
		}
		return builder;
	}
}
