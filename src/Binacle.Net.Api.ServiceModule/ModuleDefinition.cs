using Azure.Monitor.OpenTelemetry.AspNetCore;
using Binacle.Net.Api.Kernel;
using Binacle.Net.Api.ServiceModule.Configuration;
using Binacle.Net.Api.ServiceModule.Configuration.Models;
using Binacle.Net.Api.ServiceModule.Domain;
using Binacle.Net.Api.ServiceModule.Domain.Configuration.Models;
using Binacle.Net.Api.ServiceModule.Infrastructure;
using Binacle.Net.Api.ServiceModule.Services;
using ChrisMavrommatis.FluentValidation;
using ChrisMavrommatis.MinimalEndpointDefinitions;
using FluentValidation;
using HealthChecks.UI.Client;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Text;
using System.Text.Json.Serialization;

namespace Binacle.Net.Api.ServiceModule;

public static class ModuleDefinition
{
	public static void AddServiceModule(this WebApplicationBuilder builder)
	{
		Log.Information("{moduleName} module. Status {status}", "Service", "Initializing");

		builder.Configuration
			.AddJsonFile("ServiceModule/ConnectionStrings.json", optional: false, reloadOnChange: true)
			.AddJsonFile($"ServiceModule/ConnectionStrings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
			.AddEnvironmentVariables();

		// Required for local run with secrets
		if (builder.Environment.IsDevelopment())
		{
			builder.Configuration
				.AddUserSecrets<IModuleMarker>(optional: true, reloadOnChange: true);
		}

		builder.Services.AddValidatorsFromAssemblyContaining<IModuleMarker>(ServiceLifetime.Singleton, includeInternalTypes: true);

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

		builder.Configuration
			.AddJsonFile(JwtAuthOptions.FilePath, optional: false, reloadOnChange: false)
			.AddJsonFile(JwtAuthOptions.GetEnvironmentFilePath(builder.Environment.EnvironmentName), optional: true, reloadOnChange: false)
			.AddEnvironmentVariables();

		builder.Services
			.AddOptions<JwtAuthOptions>()
			.Bind(builder.Configuration.GetSection(JwtAuthOptions.SectionName))
			.ValidateFluently()
			.ValidateOnStart();

		builder.Configuration
			.AddJsonFile(DefaultsOptions.FilePath, optional: false, reloadOnChange: false)
			.AddEnvironmentVariables();

		builder.Services
			.AddOptions<DefaultsOptions>()
			.Bind(builder.Configuration.GetSection(DefaultsOptions.SectionName));

		builder.Services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

		}).AddJwtBearer(options =>
		{
			var jwtAuthOptions = builder.Configuration.GetSection(JwtAuthOptions.SectionName).Get<JwtAuthOptions>()!;

			options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
			{
				ValidIssuer = jwtAuthOptions.Issuer,
				ValidAudience = jwtAuthOptions.Audience,
				IssuerSigningKey = new SymmetricSecurityKey(
					Encoding.UTF8.GetBytes(jwtAuthOptions.TokenSecret)
				),
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				ClockSkew = TimeSpan.FromSeconds(5)
			};
		});

		builder.Services.AddEndpointsApiExplorer();

		builder.Services.AddAuthorization();

		// Register Services

		builder.Services.AddScoped<ITokenService, TokenService>();

		builder.Services
			.AddDomainLayerServices()
			.AddInfrastructureLayerServices(builder.Configuration);

		builder.Services
			.AddHealthChecks();

		builder.Services.Configure<JsonOptions>(options =>
		{
			options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
		});

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

		builder.Services.AddMinimalEndpointDefinitions<IModuleMarker>();

		builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

		builder.Services.AddRateLimiter(_ => { });
		builder.Services.ConfigureOptions<ConfigureRateLimiter>();

		//builder.Services.ConfigureRateLimiter();

		Log.Information("{moduleName} module. Status {status}", "Service", "Initialized");
	}

	public static void UseServiceModule(this WebApplication app)
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

		}).DisableRateLimiting();


		app.UseAuthentication();
		app.UseAuthorization();
		app.UseMinimalEndpointDefinitions();
		app.UseRateLimiter();
	}

	public static void ConfigureServiceModuleSwaggerUI(this SwaggerUIOptions options, WebApplication app)
	{
		ConfigureSwaggerOptions.ConfigureSwaggerUI(options, app);
	}

}
