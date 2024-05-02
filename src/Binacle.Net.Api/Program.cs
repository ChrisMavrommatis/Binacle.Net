using Asp.Versioning;
using Binacle.Net.Api.Configuration;
using Binacle.Net.Api.Configuration.Models;
using Binacle.Net.Api.ServiceModule;
using Binacle.Net.Api.Services;
using ChrisMavrommatis.Endpoints;
using ChrisMavrommatis.Features;
using ChrisMavrommatis.FluentValidation;
using ChrisMavrommatis.SwaggerExamples;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Binacle.Net.Api;

public class Program
{
	public static void Main(string[] args)
	{
		Log.Logger = new LoggerConfiguration()
			.MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
			.Enrich.FromLogContext()
			.Enrich.WithMachineName()
			.Enrich.WithThreadId()
			.WriteTo.Console()
			.CreateBootstrapLogger();

		var builder = WebApplication.CreateSlimBuilder(args);

		// Slim builder
		builder.WebHost.UseKestrelHttpsConfiguration();

		// Slim builder
		builder.WebHost.UseQuic(); //HTTP 3 support

		builder.Configuration
			.SetBasePath($"{Directory.GetCurrentDirectory()}/Config_Files");

		builder.Configuration
			.AddJsonFile("Serilog.json", optional: false, reloadOnChange: true)
			.AddJsonFile($"Serilog.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

		builder.Host.UseSerilog((context, services, loggerConfiguration) =>
		{
			loggerConfiguration
			.ReadFrom.Configuration(builder.Configuration);
		});

		Log.Information("{moduleName} module. Status {status}", "Core", "Initializing");

		builder.Configuration
			.AddJsonFile(BinPresetOptions.FilePath, optional: false, reloadOnChange: true);

		// Feature Management
		builder.Configuration
			.AddJsonFile("Features.json", optional: false, reloadOnChange: true)
			.AddJsonFile($"Features.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

		Feature.Manager = new FeatureManagerConfiguration()
			.ReadFrom.EnvironmentVariables()
			.ReadFrom.Configuration(builder.Configuration)
			.CreateManager();

		builder.Services
		   .AddOptions<BinPresetOptions>()
		   .Bind(builder.Configuration.GetSection(BinPresetOptions.SectionName))
		   .ValidateFluently()
		   .ValidateOnStart();

		builder.Services.AddValidatorsFromAssemblyContaining<IApiMarker>(ServiceLifetime.Singleton, includeInternalTypes: true);

		builder.Services.AddControllers(options =>
		{
			options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
			options.UseNamespaceRouteToken();
		});

		builder.Services.AddApiVersioning(options =>
		{
			
			options.DefaultApiVersion = ApiVersionParser.Default.Parse(v1.ApiVersion.Number);
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

		builder.Services.AddSwaggerExamples(options =>
		{
			options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
		});

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

		Log.Information("{moduleName} module. Status {status}", "Core", "Initialized");

		if (Feature.IsEnabled("SERVICE_MODULE"))
		{
			builder.AddServiceModule();
		}

		var app = builder.Build();

		// Slim builder
		app.UseHttpsRedirection();

		if (app.Environment.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
		}

		// SWAGGER_UI from environment vars
		if (Feature.IsEnabled("SWAGGER_UI"))
		{
			app.UseSwagger();
			app.UseSwaggerUI(options =>
			{
				ConfigureSwaggerOptions.ConfigureSwaggerUI(options, app);

				if (Feature.IsEnabled("SERVICE_MODULE"))
				{
					options.ConfigureServiceModuleSwaggerUI(app);
				}
			});
		}

		if (Feature.IsEnabled("SERVICE_MODULE"))
		{
			app.UseServiceModule();
		}

		app.MapControllers();

		app.Run();
	}
}
