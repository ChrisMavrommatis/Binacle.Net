using Asp.Versioning;
using Binacle.Net.Api.Configuration;
using Binacle.Net.Api.Configuration.Models;
using Binacle.Net.Api.Services;
using Binacle.Net.Api.Users;
using FluentValidation;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Binacle.Net.Api;

public class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateSlimBuilder(args);

		// Slim builder
		builder.WebHost.UseKestrelHttpsConfiguration();

		// Slim builder
		builder.WebHost.UseQuic(); //HTTP 3 support

		builder.Configuration.SetBasePath($"{Directory.GetCurrentDirectory()}/App_Data");

		builder.Configuration
			.AddJsonFile(BinPresetOptions.FilePath, optional: false, reloadOnChange: true);

		builder.Configuration
			.AddJsonFile("Serilog.json", optional: false, reloadOnChange: true)
			.AddJsonFile($"Serilog.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

		
		Log.Logger = new LoggerConfiguration()
			.ReadFrom.Configuration(builder.Configuration)
			.CreateLogger();

		builder.Host.UseSerilog();

		builder.Services
		   .AddOptions<BinPresetOptions>()
		   .Bind(builder.Configuration.GetSection(BinPresetOptions.SectionName))
		   .ValidateFluently()
		   .ValidateOnStart();

		builder.Services.AddValidatorsFromAssemblyContaining<IApiMarker>(ServiceLifetime.Singleton);

		builder.Services.AddHealthChecks();

		builder.Services.AddControllers(options =>
		{
			options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
			options.UseNamespaceRouteToken();
		});

		builder.Services.AddApiVersioning(options =>
		{
			options.DefaultApiVersion = new ApiVersion(1, 0);
			options.AssumeDefaultVersionWhenUnspecified = true;
			options.ReportApiVersions = true;
			options.ApiVersionReader = ApiVersionReader.Combine(
				new UrlSegmentApiVersionReader()
				);
		}).AddApiExplorer(options =>
		{
			options.GroupNameFormat = "'v'VVV";
			options.SubstituteApiVersionInUrl = true;
		});
		builder.Services.AddSingleton(_ => TimeProvider.System);
		builder.Services.AddSingleton<ILockerService, LockerService>();

		if (FeaturesRegistry.IsFeatureEnabled("USERS_MODULE"))
		{
			builder.AddUsersModule();
		}

		builder.Services.AddSwaggerGen();
		builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

		builder.Services.Configure<ApiBehaviorOptions>(options =>
		{
			options.SuppressModelStateInvalidFilter = true;
		});

		builder.Services.Configure<RouteOptions>(options =>
		{
			options.LowercaseQueryStrings = true;
			options.LowercaseUrls = true;
		});

		var app = builder.Build();

		// Slim builder
		app.UseHttpsRedirection();

		if (app.Environment.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
		}

		// Middleware are in order
		// Registered before Swagger because I don't want swagger to know about it
		app.MapHealthChecks("/_health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
		{
			ResultStatusCodes =
			{
				[Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy] = StatusCodes.Status200OK,
				[Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded] = StatusCodes.Status503ServiceUnavailable,
				[Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
			},
			ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
		});

		// SWAGGER_UI from environment vars
		if (FeaturesRegistry.IsFeatureEnabled("SWAGGER_UI") || app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI(options => ConfigureSwaggerOptions.ConfigureSwaggerUIOptions(app, options));
		}

		if (FeaturesRegistry.IsFeatureEnabled("USERS_MODULE"))
		{
			app.UseUsersModule();
		}

		app.MapControllers();

		app.Run();
	}
}
