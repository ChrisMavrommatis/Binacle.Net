using Azure.Data.Tables;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Binacle.Net.Api.ServiceModule.Configuration;
using Binacle.Net.Api.ServiceModule.Configuration.Models;
using Binacle.Net.Api.ServiceModule.Data.Repositories;
using Binacle.Net.Api.ServiceModule.EndpointDefinitions;
using Binacle.Net.Api.ServiceModule.Services;
using ChrisMavrommatis.Api.MinimalEndpoints;
using FluentValidation;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Serilog;
using System.Text;
using System.Threading.RateLimiting;

namespace Binacle.Net.Api.ServiceModule;

public static class ModuleDefinition
{
	public static void AddServiceModule(this WebApplicationBuilder builder)
	{
		// overwrite default
		Log.Logger = new LoggerConfiguration()
			.ReadFrom.Configuration(builder.Configuration)
			.WriteTo.ApplicationInsights(TelemetryConverter.Traces)
			.CreateLogger();

		Log.Logger.Information("Starting up with Service Module");


		builder.Configuration
			.AddJsonFile(JwtAuthOptions.FilePath, optional: false, reloadOnChange: false)
			.AddJsonFile(JwtAuthOptions.GetEnvironmentFilePath(builder.Environment.EnvironmentName), optional: true, reloadOnChange: false)
			.AddEnvironmentVariables(JwtAuthOptions.SectionName);

		builder.Services
			.AddOptions<JwtAuthOptions>()
			.Bind(builder.Configuration.GetSection(JwtAuthOptions.SectionName))
			.ValidateFluently()
			.ValidateOnStart();

		var jwtAuthOptions = builder.Configuration.GetSection(JwtAuthOptions.SectionName).Get<JwtAuthOptions>();

		builder.Services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
		}).AddJwtBearer(options =>
		{
			options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				ValidIssuer = jwtAuthOptions.Issuer,
				ValidAudience = jwtAuthOptions.Audience,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAuthOptions.TokenSecret))
			};
		});

		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddValidatorsFromAssemblyContaining<IModuleMarker>(ServiceLifetime.Singleton, includeInternalTypes: true);
		builder.Services.AddAuthorization();

		// Register Services
		builder.Services.AddScoped<IUserRepository, UserRepository>();
		builder.Services.AddScoped<ITokenService, TokenService>();
		builder.Services.AddScoped<IAuthService, AuthService>();
		builder.Services.AddSingleton<TableServiceClient>(sp =>
		{
			var connectionString = Environment.GetEnvironmentVariable("STORAGEACCOUNT_CONNECTION_STRING");
			return new TableServiceClient(connectionString);
		});

		builder.Services
			.AddHealthChecks();

		builder.Services.AddOpenTelemetry()
			.WithMetrics(configure =>
			{
				configure.AddRuntimeInstrumentation()
				.AddMeter(
					"Microsoft.AspNetCore.Hosting",
					"Microsoft.AspNetCore.Server.Kestrel"
					)
				.AddMeter("Microsoft.AspNetCore.RateLimiting");

			}).WithTracing(configure =>
			{
				if (builder.Environment.IsDevelopment())
				{
					configure.SetSampler<AlwaysOnSampler>();
				}

				configure.AddAspNetCoreInstrumentation();
			}).UseAzureMonitor();

		var endpointDefinitions = new List<IEndpointDefinition>()
		{
			new AuthEndpointsDefinition()
		};

		if (builder.Environment.IsDevelopment())
		{
			endpointDefinitions.Add(new UsersEndpointsDefinition());
		}

		builder.Services.AddSingleton(endpointDefinitions as IReadOnlyCollection<IEndpointDefinition>);
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
	}

	public static void UseServiceModule(this WebApplication app)
	{
		// Middleware are in order
		// Registered before Swagger because I don't want swagger to know about it
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

		var endpointDefinitions = app.Services.GetRequiredService<IReadOnlyCollection<IEndpointDefinition>>();
		foreach (var endpointDefinition in endpointDefinitions)
		{
			endpointDefinition.DefineEndpoints(app);
		}

		app.UseRateLimiter();
	}
}
