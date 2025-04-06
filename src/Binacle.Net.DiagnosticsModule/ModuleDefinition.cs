using Binacle.Net.DiagnosticsModule.ExtensionMethods;
using Binacle.Net.Kernel.Logs.Models;
using Binacle.Net.DiagnosticsModule.Configuration.Models;
using Binacle.Net.DiagnosticsModule.Middleware;
using FluentValidation;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;

namespace Binacle.Net.DiagnosticsModule;

public static class ModuleDefinition
{
	public static void BootstrapLogger(this WebApplicationBuilder builder)
	{
		builder.Logging.ClearProviders();

		Log.Logger = new LoggerConfiguration()
			.MinimumLevel.Information()
			.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
			.MinimumLevel.Override("Azure.Core", LogEventLevel.Warning)
			.WriteTo.Console()
			.Enrich.FromLogContext()
			.Enrich.WithMachineName()
			.Enrich.WithProcessId()
			.Enrich.WithThreadId()
			.Enrich.WithBinacleVersion()
			.CreateBootstrapLogger();
	}
	
	public static void AddDiagnosticsModule(this WebApplicationBuilder builder)
	{
		Log.Information("{moduleName} module. Status {status}", "Diagnostics", "Initializing");

		// Required for local run with secrets
		if (builder.Environment.IsDevelopment())
		{
			builder.Configuration
				.AddUserSecrets<IModuleMarker>(optional: true, reloadOnChange: true);
		}

		// Logging
		builder.AddJsonConfiguration(
			filePath: "DiagnosticsModule/Serilog.json",
			environmentFilePath: $"DiagnosticsModule/Serilog.{builder.Environment.EnvironmentName}.json",
			optional: false,
			reloadOnChange: true
		);

		// overwrite default logger
		builder.Host.UseSerilog((context, services, loggerConfiguration) =>
		{
			loggerConfiguration
				.ReadFrom.Configuration(builder.Configuration)
				.Enrich.WithBinacleVersion();
		}, writeToProviders: true);

		builder.Services.AddValidatorsFromAssemblyContaining<IModuleMarker>(ServiceLifetime.Singleton,
			includeInternalTypes: true);

		// Open Telemetry
		builder.AddValidatableJsonConfigurationOptions<OpenTelemetryConfigurationOptions>();

		var openTelemetryOptions = builder.Configuration.GetConfigurationOptions<OpenTelemetryConfigurationOptions>();

		if (openTelemetryOptions?.IsEnabled() ?? false)
		{
			Log.Information("OpenTelemetry is enabled.");
			var openTelemetryBuilder = builder.Services
				.AddOpenTelemetry()
				.ConfigureResource(resourceBuilder =>
				{
					resourceBuilder.AddService(
						serviceName: "Binacle.Net",
						serviceVersion: Environment.GetEnvironmentVariable("BINACLE_VERSION") ?? "Unknown",
						serviceNamespace: openTelemetryOptions.ServiceNamespace,
						serviceInstanceId: openTelemetryOptions.ServiceInstanceId
					)
					.AddOptionalAdditionalAttributes(openTelemetryOptions);
				})
				.WithMetrics(meterBuilder =>
				{
					meterBuilder
						.ConfigureResource(config =>
							config.AddOptionalAdditionalAttributes(openTelemetryOptions.Metrics)
						)
						.AddRuntimeInstrumentation()
						.AddAspNetCoreInstrumentation()
						.AddMeters(openTelemetryOptions.Metrics.AdditionalMeters);
				})
				.WithTracing(traceBuilder =>
				{
					traceBuilder
						.ConfigureResource(config =>
							config.AddOptionalAdditionalAttributes(openTelemetryOptions.Tracing)
						)
						.AddAspNetCoreInstrumentation(options =>
							options.ConfigureAspNetCoreInstrumentation(openTelemetryOptions.Tracing)
						)
						.AddHttpClientInstrumentation(options =>
							options.ConfigureHttpClientInstrumentation(openTelemetryOptions.Tracing)
						)
						.AddSources(openTelemetryOptions.Tracing.AdditionalSources);
				})
				.WithLogging(logBuilder =>
				{
					logBuilder.ConfigureResource(config =>
						config.AddOptionalAdditionalAttributes(openTelemetryOptions.Logging)
					);
				});

			if (openTelemetryOptions.Otlp.Enabled)
			{
				Log.Information("Using {Provider} with OpenTelemetry.", "Otlp Exporter");
				openTelemetryBuilder.UseOtlpExporter(openTelemetryOptions.Otlp);
			}

			if (openTelemetryOptions.AzureMonitor.Enabled)
			{
				Log.Information("Using {Provider} with OpenTelemetry.", "Azure Monitor");
				openTelemetryBuilder.UseAzureMonitor(openTelemetryOptions.AzureMonitor);
			}
		}

		// Packing Logs
		builder.AddValidatableJsonConfigurationOptions<PackingLogsConfigurationOptions>();

		var packingLogsOptionsIsEnabled = builder.Configuration
			.GetSection(PackingLogsConfigurationOptions.SectionName)
			.GetValue<bool>(nameof(PackingLogsConfigurationOptions.Enabled));
		
		if (packingLogsOptionsIsEnabled)
		{
			builder.Services
				.AddOptionsBasedLogProcessor<LegacyFittingLogChannelRequest>(
					optionsSelector: options => options.LegacyFitting!,
					logFormatter: request => request.ConvertToLogObject());

			builder.Services
				.AddOptionsBasedLogProcessor<LegacyPackingLogChannelRequest>(
					optionsSelector: options => options.LegacyPacking!,
					logFormatter: request => request.ConvertToLogObject());

			builder.Services
				.AddOptionsBasedLogProcessor<PackingLogChannelRequest>(
					optionsSelector: options => options.Packing!,
					logFormatter: request => request.ConvertToLogObject());
		}

		// Health Checks
		builder.AddValidatableJsonConfigurationOptions<HealthCheckConfigurationOptions>();

		// Add health checks
		builder.Services
			.AddHealthChecks();

		Log.Information("{moduleName} module. Status {status}", "Diagnostics", "Initialized");
	}

	public static void UseDiagnosticsModule(this WebApplication app)
	{
		var healthChecksOptions = app.Services.GetRequiredService<IOptions<HealthCheckConfigurationOptions>>();

		if (healthChecksOptions.Value.Enabled)
		{
			app.UseMiddleware<HealthChecksProtectionMiddleware>();

			app.MapHealthChecks(healthChecksOptions.Value.Path!,
				new HealthCheckOptions
				{
					ResultStatusCodes =
					{
						[HealthStatus.Healthy] = StatusCodes.Status200OK,
						[HealthStatus.Degraded] = StatusCodes.Status200OK,
						[HealthStatus.Unhealthy] =
							StatusCodes.Status503ServiceUnavailable
					},
					ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
					Predicate = (check) =>
					{
						if (healthChecksOptions.Value.RestrictedChecks is null ||
						    healthChecksOptions.Value.RestrictedChecks.Length == 0)
						{
							return true;
						}

						return healthChecksOptions.Value.RestrictedChecks.Contains(check.Name);
					}
				});
		}
	}
}
