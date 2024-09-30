using Azure.Monitor.OpenTelemetry.AspNetCore;
using Binacle.Net.Api.DiagnosticsModule.Configuration.Models;
using Binacle.Net.Api.DiagnosticsModule.Middleware;
using Binacle.Net.Api.Kernel;
using ChrisMavrommatis.FluentValidation;
using FluentValidation;
using HealthChecks.UI.Client;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Serilog;

namespace Binacle.Net.Api.DiagnosticsModule;

public static class ModuleDefinition
{
	public static void BootstrapLogger()
	{
		Log.Logger = new LoggerConfiguration()
			.MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
			.Enrich.FromLogContext()
			.Enrich.WithMachineName()
			.Enrich.WithThreadId()
			.WriteTo.Console()
			.CreateBootstrapLogger();
	}

	public static void AddDiagnosticsModule(this WebApplicationBuilder builder)
	{
		Log.Information("{moduleName} module. Status {status}", "Diagnostics", "Initializing");

		builder.Configuration
			.AddJsonFile("DiagnosticsModule/ConnectionStrings.json", optional: false, reloadOnChange: true)
			.AddJsonFile($"DiagnosticsModule/ConnectionStrings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
			.AddEnvironmentVariables();

		// Required for local run with secrets
		if (builder.Environment.IsDevelopment())
		{
			builder.Configuration
				.AddUserSecrets<IModuleMarker>(optional: true, reloadOnChange: true);
		}

		// Logging
		builder.Configuration
			.AddJsonFile("DiagnosticsModule/Serilog.json", optional: false, reloadOnChange: true)
			.AddJsonFile($"DiagnosticsModule/Serilog.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

		var applicationInsightsConnectionString = builder.Configuration.GetConnectionStringWithEnvironmentVariableFallback(
			"ApplicationInsights",
			"APPLICATIONINSIGHTS_CONNECTION_STRING"
			);


		// overwrite default logger
		builder.Host.UseSerilog((context, services, loggerConfiguration) =>
		{
			loggerConfiguration
			.ReadFrom.Configuration(builder.Configuration);

			if (applicationInsightsConnectionString is not null)
			{
				loggerConfiguration.WriteTo.ApplicationInsights(
					services.GetRequiredService<TelemetryConfiguration>(),
					TelemetryConverter.Traces
				);
			}
		});

		builder.Services.AddValidatorsFromAssemblyContaining<IModuleMarker>(ServiceLifetime.Singleton, includeInternalTypes: true);

		// Health Checks
		builder.Configuration
			.AddJsonFile(HealthCheckConfigurationOptions.FilePath, optional: false, reloadOnChange: true)
			.AddJsonFile(HealthCheckConfigurationOptions.GetEnvironmentFilePath(builder.Environment.EnvironmentName), optional: true, reloadOnChange: true)
			.AddEnvironmentVariables();

		builder.Services
			.AddOptions<HealthCheckConfigurationOptions>()
			.Bind(builder.Configuration.GetSection(HealthCheckConfigurationOptions.SectionName))
			.ValidateFluently()
			.ValidateOnStart();

		// Add health checks
		builder.Services
			.AddHealthChecks();


		// Add OpenTelemetry
		var openTelemetryBuilder = builder.Services
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
			});


		if (applicationInsightsConnectionString is not null)
		{
			builder.Services.AddApplicationInsightsTelemetry(options =>
			{
				options.ConnectionString = applicationInsightsConnectionString;
				options.EnableQuickPulseMetricStream = false;
			});


			openTelemetryBuilder.UseAzureMonitor(configure =>
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
		var healthChecksOptions = app.Services.GetRequiredService<IOptions<HealthCheckConfigurationOptions>>();

		if (healthChecksOptions.Value.Enabled)
		{
			app.UseMiddleware<HealthChecksProtectionMiddleware>();

			app.MapHealthChecks(healthChecksOptions.Value.Path, new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
			{
				ResultStatusCodes =
				{
					[Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy] = StatusCodes.Status200OK,
					[Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded] = StatusCodes.Status200OK,
					[Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
				},
				ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
				Predicate = (check) =>
				{
					if(healthChecksOptions.Value.RestrictedChecks is null || healthChecksOptions.Value.RestrictedChecks.Length == 0)
					{
						return true;
					}

					return healthChecksOptions.Value.RestrictedChecks.Contains(check.Name);
				}
			});
		}


	}
}
