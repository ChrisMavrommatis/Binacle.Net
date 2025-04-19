using System.Security.Claims;
using System.Text;
using Binacle.Net.Kernel.OpenApi.ExtensionsMethods;
using Binacle.Net.ServiceModule.Application;
using Binacle.Net.ServiceModule.Application.Authentication.Configuration;
using Binacle.Net.ServiceModule.Configuration.Models;
using Binacle.Net.ServiceModule.Domain.Accounts.Models;
using Binacle.Net.ServiceModule.Infrastructure;
using Binacle.Net.ServiceModule.Models;
using Binacle.Net.ServiceModule.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace Binacle.Net.ServiceModule;

public static class ModuleDefinition
{
	public static void AddServiceModule(this WebApplicationBuilder builder)
	{
		Log.Information("{moduleName} module. Status {status}", "Service", "Initializing");

		builder.AddJsonConfiguration(
			filePath: "ServiceModule/ConnectionStrings.json",
			environmentFilePath:$"ServiceModule/ConnectionStrings.{builder.Environment.EnvironmentName}.json",
			optional: false,
			reloadOnChange: true
		);
		
		// Required for local run with secrets
		if (builder.Environment.IsDevelopment())
		{
			builder.Configuration
				.AddUserSecrets<IModuleMarker>(optional: true, reloadOnChange: true);
		}

		builder.Services.AddValidatorsFromAssemblyContaining<IModuleMarker>(ServiceLifetime.Singleton, includeInternalTypes: true);

		builder.AddValidatableJsonConfigurationOptions<RateLimiterConfigurationOptions>();

		builder.AddValidatableJsonConfigurationOptions<JwtAuthOptions>();
		
		builder.Services.AddOpenApiDocumentsFromAssemblyContaining<IModuleMarker>();

		builder.Services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

		}).AddJwtBearer(options =>
		{
			var jwtAuthOptions = builder.Configuration.GetSection(JwtAuthOptions.SectionName).Get<JwtAuthOptions>()!;

			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidIssuer = jwtAuthOptions.Issuer,
				ValidAudience = jwtAuthOptions.Audience,
				IssuerSigningKey = new SymmetricSecurityKey(
					Encoding.UTF8.GetBytes(jwtAuthOptions.TokenSecret!)
				),
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				ClockSkew = TimeSpan.FromSeconds(5)
			};
		});

		builder.Services.AddAuthorization(options =>
		{
			options.AddPolicy("Admin", policyBuilder =>
			{
				policyBuilder.RequireAuthenticatedUser();
				policyBuilder.RequireClaim(ClaimTypes.Role, AccountRole.Admin.ToString());
			});
		});


		builder
			.AddApplication()
			.AddInfrastructure();
		
		builder.Services.AddRateLimiter(options =>
		{
			options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

			options.AddPolicy<string, AnonymousRateLimitingPolicy>("Anonymous");
			options.AddPolicy<string, AuthRateLimitingPolicy>("Auth");
		});
		
		builder.Services.Configure<FeatureOptions>(options =>
		{
			options.AddFeature("RateLimiter");
		});

		Log.Information("{moduleName} module. Status {status}", "Service", "Initialized");
	}

	public static void UseServiceModule(this WebApplication app)
	{
		app.UseAuthentication();
		app.UseAuthorization();
		app.UseRateLimiter();
		app.RegisterEndpointsFromAssemblyContaining<IModuleMarker>();
	}
}
