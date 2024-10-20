using Asp.Versioning;
using Binacle.Net.Api.Configuration;
using Binacle.Net.Api.Configuration.Models;
using Binacle.Net.Api.DiagnosticsModule;
using Binacle.Net.Api.ServiceModule;
using Binacle.Net.Api.Services;
using Binacle.Net.Api.UIModule;
using ChrisMavrommatis.Endpoints;
using ChrisMavrommatis.Features;
using ChrisMavrommatis.FluentValidation;
using ChrisMavrommatis.StartupTasks;
using ChrisMavrommatis.SwaggerExamples;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Text.Json.Serialization;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Binacle.Net.Api;

public class Program
{
	public static async Task Main(string[] args)
	{
		Binacle.Net.Api.DiagnosticsModule.ModuleDefinition.BootstrapLogger();

		var builder = WebApplication.CreateSlimBuilder(args);

		// Slim builder
		builder.WebHost.UseKestrelHttpsConfiguration();

		// Slim builder
		builder.WebHost.UseQuic(); //HTTP 3 support

		builder.Configuration
			.SetBasePath($"{Directory.GetCurrentDirectory()}/Config_Files");

		builder.Configuration
			.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
			.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

		Log.Information("{moduleName} module. Status {status}", "Core", "Initializing");

		builder.Configuration
			.AddJsonFile(BinPresetOptions.FilePath, optional: false, reloadOnChange: true);

		// Feature Management
		builder.Configuration
			.AddJsonFile("Features.json", optional: false, reloadOnChange: true)
			.AddJsonFile($"Features.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

		Feature.Manager = new FeatureManagerConfiguration()
			.ReadFrom.Configuration(builder.Configuration)
			.ReadFrom.EnvironmentVariables()
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

		}).AddJsonOptions(options =>
		{
			options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
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
			// ignore null
			options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
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

		builder.AddDiagnosticsModule();

		if (Feature.IsEnabled("SERVICE_MODULE"))
		{
			builder.AddServiceModule();
		}

		if (Feature.IsEnabled("UI_MODULE"))
		{
			builder.AddUIModule();
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

		app.UseDiagnosticsModule();

		if (Feature.IsEnabled("SERVICE_MODULE"))
		{
			app.UseServiceModule();
		}

		if (Feature.IsEnabled("UI_MODULE"))
		{
			app.UseUIModule();
		}

		app.MapControllers();

		await app.RunStartupTasksAsync();
		await app.RunAsync();
	}
}
