using Azure.Data.Tables;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Binacle.Net.Api.ServiceModule.Configuration;
using Binacle.Net.Api.ServiceModule.Configuration.Models;
using Binacle.Net.Api.ServiceModule.Data.Repositories;
using Binacle.Net.Api.ServiceModule.Services;
using ChrisMavrommatis.FluentValidation;
using ChrisMavrommatis.MinimalEndpointDefinitions;
using FluentValidation;
using HealthChecks.UI.Client;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Text;
using System.Threading.RateLimiting;

namespace Binacle.Net.Api.ServiceModule;

public static class ModuleDefinition
{
	public static void AddServiceModule(this WebApplicationBuilder builder)
	{
		Log.Information("{moduleName} module. Status {status}", "Service", "Initializing");

		builder.Configuration
			.AddUserSecrets<IModuleMarker>(optional: true, reloadOnChange: true);

		var applicationInsightsConnectionString = GetConnectionStringWithEnvironmentVariableFallback(
			builder.Configuration,
			"ApplicationInsights",
			"APPLICATIONINSIGHTS_CONNECTION_STRING"
			);


		if (!string.IsNullOrEmpty(applicationInsightsConnectionString))
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
			.AddEnvironmentVariables()
			.AddUserSecrets<IModuleMarker>(optional: true, reloadOnChange: true);

		builder.Services
			.AddOptions<JwtAuthOptions>()
			.Bind(builder.Configuration.GetSection(JwtAuthOptions.SectionName))
			.ValidateFluently()
			.ValidateOnStart();

		var jwtAuthOptions = builder.Configuration.GetSection(JwtAuthOptions.SectionName).Get<JwtAuthOptions>();
		if(jwtAuthOptions is null)
		{
			Log.Error("Required config {Config} not found during startup", nameof(JwtAuthOptions));
			throw new System.ArgumentNullException(nameof(JwtAuthOptions), "Config is required during startup");
		}

		builder.Services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

		}).AddJwtBearer(options =>
		{
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
		builder.Services.AddValidatorsFromAssemblyContaining<IModuleMarker>(ServiceLifetime.Singleton, includeInternalTypes: true);
		builder.Services.AddAuthorization();

		// Register Services
		builder.Services.AddScoped<IUserRepository, UserRepository>();
		builder.Services.AddScoped<ITokenService, TokenService>();
		builder.Services.AddScoped<IUserManagerService, UserManagerService>();
		builder.Services.AddSingleton<TableServiceClient>(sp =>
		{
			var connectionString = GetConnectionStringWithEnvironmentVariableFallback(
				builder.Configuration,
				"AzureStorage",
				"AZURESTORAGE_CONNECTION_STRING");

			if (string.IsNullOrWhiteSpace(connectionString))
			{
				throw new InvalidOperationException("AzureStorage connection string is missing");
			}

			return new TableServiceClient(connectionString);
		});

		builder.Services
			.AddHealthChecks();


		if (!string.IsNullOrEmpty(applicationInsightsConnectionString))
		{
			builder.Services.AddApplicationInsightsTelemetry(options =>
			{
				options.ConnectionString = applicationInsightsConnectionString;
			});


			//builder.Services.AddOpenTelemetry()
			//	.WithMetrics(configure =>
			//	{
			//		configure
			//		.AddRuntimeInstrumentation()
			//		.AddMeter("Microsoft.AspNetCore.Hosting")
			//		.AddMeter("Microsoft.AspNetCore.Server.Kestrel")
			//		.AddMeter("Microsoft.AspNetCore.RateLimiting");

			//	}).WithTracing(configure =>
			//	{
			//		configure.AddAspNetCoreInstrumentation();

			//	}).UseAzureMonitor(configure =>
			//	{
			//		var samplingRatio = Environment.GetEnvironmentVariable("AZUREMONITOR_SAMPLING_RATIO");
			//		if (samplingRatio != null && float.TryParse(samplingRatio, out var ratio))
			//		{
			//			configure.SamplingRatio = ratio;
			//		}

			//		configure.ConnectionString = applicationInsightsConnectionString;
			//	});
		}

		builder.Services.AddMinimalEndpointDefinitions<IModuleMarker>();
		builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

		builder.Services.AddRateLimiter(options =>
		{
			options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
			{
				var user = httpContext.User;
				if (user?.Identity?.IsAuthenticated ?? false)
				{
					return RateLimitPartition.GetNoLimiter("Authenticated");
				}

				return RateLimitPartition.GetFixedWindowLimiter("Anonymous", _ =>
				new FixedWindowRateLimiterOptions
				{
					Window = TimeSpan.FromSeconds(60),
					PermitLimit = 10,
					QueueLimit = 0,
					QueueProcessingOrder = QueueProcessingOrder.NewestFirst
				});
			});
			options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
		});

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

	private static string? GetConnectionStringWithEnvironmentVariableFallback(
		ConfigurationManager configuration,
		string name,
		string variable
		)
	{
		var connectionString = configuration.GetConnectionString(name);

		if (!string.IsNullOrWhiteSpace(connectionString))
		{
			Log.Information("Connection String {connectionString} found in {location}", name, "Configuration File");
			return connectionString;
		}

		connectionString = Environment.GetEnvironmentVariable(variable);

		if (!string.IsNullOrWhiteSpace(connectionString))
		{
			Log.Information("Connection String {connectionString} found in {location}", name, $"Environment Variable: {variable}");
			return connectionString;
		}

		Log.Warning("Connection String {connectionString} not found", name);
		return null;
	}

}
