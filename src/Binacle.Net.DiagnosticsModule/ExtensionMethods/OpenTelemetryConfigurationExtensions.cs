using Azure.Monitor.OpenTelemetry.AspNetCore;
using Binacle.Net.DiagnosticsModule.Configuration.Models;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Instrumentation.Http;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Binacle.Net.DiagnosticsModule.ExtensionMethods;

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

	public static void UseOtlpExporter(
		this OpenTelemetryBuilder builder,
		Configuration.Models.OtlpExporterConfigurationOptions otlp
	)
	{
		if (!string.IsNullOrEmpty(otlp.Endpoint))
		{
			builder.UseOtlpExporter(
				otlp.GetOtlpExportProtocol(),
				new Uri(otlp.Endpoint!)
			);
			return;
		}

		// Expected config to be passed through environemnt variables
		builder.UseOtlpExporter();
	}

	public static void UseAzureMonitor(
		this OpenTelemetryBuilder builder,
		AzureMonitorConfigurationOptions options
	)
	{
		builder.UseAzureMonitor(azureMonitorOptions =>
		{
			if (!string.IsNullOrEmpty(options.ConnectionString))
			{
				azureMonitorOptions.ConnectionString = options.ConnectionString!;
			}
			azureMonitorOptions.SamplingRatio = options.SamplingRatio;
			azureMonitorOptions.EnableLiveMetrics = options.EnableLiveMetrics;
		});
	}

	public static ResourceBuilder AddOptionalAdditionalAttributes<T>(
		this ResourceBuilder builder,
		T model
	)
		where T : IOpenTelemetryAttributes
	{
		if (model.AdditionalAttributes?.Any() ?? false)
		{
			builder.AddAttributes(model.AdditionalAttributes!);
		}

		return builder;
	}
}
