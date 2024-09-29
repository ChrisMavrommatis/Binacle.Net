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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
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

		builder.Configuration
			.AddJsonFile(DefaultsOptions.FilePath, optional: false, reloadOnChange: false)
			.AddEnvironmentVariables();

		builder.Services
			.AddOptions<DefaultsOptions>()
			.Bind(builder.Configuration.GetSection(DefaultsOptions.SectionName));


		builder.Configuration
			.AddJsonFile(RateLimiterConfigurationOptions.FilePath, optional: false, reloadOnChange: false)
			.AddJsonFile(RateLimiterConfigurationOptions.GetEnvironmentFilePath(builder.Environment.EnvironmentName), optional: true, reloadOnChange: false)
			.AddEnvironmentVariables();

		builder.Services
			.AddOptions<RateLimiterConfigurationOptions>()
			.Bind(builder.Configuration.GetSection(RateLimiterConfigurationOptions.SectionName))
			.ValidateFluently()
			.ValidateOnStart();


		builder.Configuration
			.AddJsonFile(JwtAuthOptions.FilePath, optional: false, reloadOnChange: false)
			.AddJsonFile(JwtAuthOptions.GetEnvironmentFilePath(builder.Environment.EnvironmentName), optional: true, reloadOnChange: false)
			.AddEnvironmentVariables();

		builder.Services
			.AddOptions<JwtAuthOptions>()
			.Bind(builder.Configuration.GetSection(JwtAuthOptions.SectionName))
			.ValidateFluently()
			.ValidateOnStart();

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

		builder.Services.AddMinimalEndpointDefinitions<IModuleMarker>();

		builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

		builder.Services.AddRateLimiter(_ => { });
		builder.Services.ConfigureOptions<ConfigureRateLimiter>();

		Log.Information("{moduleName} module. Status {status}", "Service", "Initialized");
	}

	public static void UseServiceModule(this WebApplication app)
	{
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
