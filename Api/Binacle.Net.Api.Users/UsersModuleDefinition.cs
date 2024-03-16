using Azure.Data.Tables;
using Binacle.Net.Api.Users.Configuration;
using Binacle.Net.Api.Users.Configuration.Models;
using Binacle.Net.Api.Users.Data.Services;
using Binacle.Net.Api.Users.EndpointDefinitions;
using Binacle.Net.Api.Users.Properties;
using Binacle.Net.Api.Users.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;

namespace Binacle.Net.Api.Users;

public static class UsersModuleDefinition
{
	public static void AddUsersModule(this WebApplicationBuilder builder)
	{
		builder.Configuration
			.AddJsonFile(JwtAuthOptions.FilePath, optional: false, reloadOnChange: false)
			.AddJsonFile(JwtAuthOptions.GetEnvironmentFilePath(builder.Environment.EnvironmentName), optional: true, reloadOnChange: false)
			.AddEnvironmentVariables();

		builder.Services
			.AddOptions<JwtAuthOptions>()
			.Bind(builder.Configuration.GetSection(JwtAuthOptions.SectionName));

		var jwtAuthOptions = builder.Configuration.GetSection(JwtAuthOptions.SectionName).Get<JwtAuthOptions>();
		if (!jwtAuthOptions.IsConfigured())
		{
			throw new System.ApplicationException("JWT Not Configured");
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
		builder.Services.AddScoped<ITokenService, TokenService>();
		builder.Services.AddSingleton<TableServiceClient>(sp =>
		{
			var connectionString = Environment.GetEnvironmentVariable("STORAGEACCOUNT_CONNECTION_STRING");
			return new TableServiceClient(connectionString);
		});

		builder.Services.AddScoped<IAuthService, AuthService>();

		var endpointDefinitions = new List<IEndpointDefinition>()
		{
			new AuthEndpointsDefinition()
		};

		builder.Services.AddSingleton(endpointDefinitions as IReadOnlyCollection<IEndpointDefinition>);
		builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

		builder.Services.AddRateLimiter(options =>
		{
			options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
			{
				var user = httpContext.User;
				if(user?.Identity?.IsAuthenticated ?? false)
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

	public static void UseUsersModule(this WebApplication app)
	{
		app.UseAuthentication();
		app.UseAuthorization();
		var endpointDefinitions = app.Services.GetRequiredService<IReadOnlyCollection<IEndpointDefinition>>();
		foreach(var endpointDefinition in endpointDefinitions)
		{
			endpointDefinition.DefineEndpoints(app);
		}

		app.UseRateLimiter();
	}
}
