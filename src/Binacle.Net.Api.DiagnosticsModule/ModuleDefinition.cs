﻿using Azure.Monitor.OpenTelemetry.AspNetCore;
using Binacle.Net.Api.Kernel;
using HealthChecks.UI.Client;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Serilog;

namespace Binacle.Net.Api.DiagnosticsModule;

public static class ModuleDefinition
{
	public static void AddDiagnosticsModule(this WebApplicationBuilder builder)
	{
		Log.Information("{moduleName} module. Status {status}", "Diagnostics", "Initializing");


		builder.Configuration
			.AddJsonFile("DiagnosticsModule/ConnectionStrings.json", optional: false, reloadOnChange: true)
			.AddJsonFile($"DiagnosticsModule/ConnectionStrings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
			.AddEnvironmentVariables();

		builder.Configuration
			.AddJsonFile("DiagnosticsModule/Serilog.json", optional: false, reloadOnChange: true)
			.AddJsonFile($"DiagnosticsModule/Serilog.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

		builder.Host.UseSerilog((context, services, loggerConfiguration) =>
		{
			loggerConfiguration
			.ReadFrom.Configuration(builder.Configuration);
		});

		var applicationInsightsConnectionString = builder.Configuration.GetConnectionStringWithEnvironmentVariableFallback(
			"ApplicationInsights",
			"APPLICATIONINSIGHTS_CONNECTION_STRING"
			);

		if (applicationInsightsConnectionString is not null)
		{
			// overwrite default
			builder.Host.UseSerilog((context, services, loggerConfiguration) =>
			{
				loggerConfiguration
				.ReadFrom.Configuration(builder.Configuration)
				.WriteTo.ApplicationInsights(
					services.GetRequiredService<TelemetryConfiguration>(),
					TelemetryConverter.Traces
					);
			});
		}

		builder.Services
			.AddHealthChecks();

		var optl = builder.Services
			.AddOpenTelemetry()
			//.ConfigureResource(resource =>
			//{
			//	resource.AddService(serviceName: "Binacle-Net");
			//})
			.WithMetrics(configure =>
			{
				configure
					.AddRuntimeInstrumentation()
					.AddMeter("Microsoft.AspNetCore.Hosting")
					.AddMeter("Microsoft.AspNetCore.Server.Kestrel")
					.AddMeter("Microsoft.AspNetCore.RateLimiting");
			}).WithTracing(configure =>
			{
				configure.AddAspNetCoreInstrumentation();
				//configure.AddConsoleExporter();
			});


		if (applicationInsightsConnectionString is not null)
		{
			builder.Services.AddApplicationInsightsTelemetry(options =>
			{
				options.ConnectionString = applicationInsightsConnectionString;
				options.EnableQuickPulseMetricStream = false;
			});


			optl.UseAzureMonitor(configure =>
			{
				var samplingRatio = Environment.GetEnvironmentVariable("AZUREMONITOR_SAMPLING_RATIO");
				if (samplingRatio != null && float.TryParse(samplingRatio, out var ratio))
				{
					configure.SamplingRatio = ratio;
				}

				configure.ConnectionString = applicationInsightsConnectionString;
			});
		}

		Log.Information("{moduleName} module. Status {status}", "Diagnostics", "Initialized");
	}

	public static void UseDiagnosticsModule(this WebApplication app)
	{
		// Middleware are in order
		app.MapHealthChecks("/_health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
		{
			ResultStatusCodes =
			{
				[Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy] = StatusCodes.Status200OK,
				[Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded] = StatusCodes.Status200OK,
				[Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
			},
			ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse

		});

	}


}
