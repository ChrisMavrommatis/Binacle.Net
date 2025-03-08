using Binacle.Net.Api.DiagnosticsModule.Configuration.Models;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Instrumentation.Http;
using OpenTelemetry.Metrics;
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
		if (!string.IsNullOrEmpty(configurationOptions.Protocol))
		{
			options.Protocol = configurationOptions.Protocol! switch
			{
				"grpc" => OtlpExportProtocol.Grpc,
				"httpProtobuf" => OtlpExportProtocol.HttpProtobuf,
				_ => throw new ArgumentOutOfRangeException(nameof(configurationOptions.Protocol), configurationOptions.Protocol, null)
			};
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
}
