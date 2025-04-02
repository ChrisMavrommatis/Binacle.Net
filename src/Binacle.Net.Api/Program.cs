using Binacle.Net.Api.Configuration.Models;
using Binacle.Net.Api.DiagnosticsModule;
using Binacle.Net.Api.ExtensionMethods;
using Binacle.Net.Api.Kernel.OpenApi.ExtensionsMethods;
using Binacle.Net.Api.ServiceModule;
using Binacle.Net.Api.Services;
using Binacle.Net.Api.UIModule;
using ChrisMavrommatis.Features;
using ChrisMavrommatis.StartupTasks;
using FluentValidation;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Serilog;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Binacle.Net.Api;

public class Program
{
	public static async Task Main(string[] args)
	{
		var builder = WebApplication.CreateSlimBuilder(args);
		builder.BootstrapLogger();

		// Slim builder
		builder.WebHost.UseKestrelHttpsConfiguration();

		// Slim builder
		builder.WebHost.UseQuic(); //HTTP 3 support

		builder.Configuration
			.SetBasePath($"{Directory.GetCurrentDirectory()}/Config_Files");

		builder.Configuration.AddEnvironmentVariables();
		
		builder.AddJsonConfiguration(
			filePath: "appsettings.json",
			environmentFilePath: $"appsettings.{builder.Environment.EnvironmentName}.json",
			optional: true,
			reloadOnChange: true
		);
		
		Log.Information("{moduleName} module. Status {status}", "Core", "Initializing");

		builder.AddValidatableJsonConfigurationOptions<BinPresetOptions>();

		// Feature Management
		Feature.Manager = new FeatureManagerConfiguration()
			.ReadFrom.Configuration(builder.Configuration)
			.ReadFrom.EnvironmentVariables()
			.CreateManager();
		
		builder.Services.AddValidatorsFromAssemblyContaining<IApiMarker>(ServiceLifetime.Singleton, includeInternalTypes: true);
		builder.Services.AddEndpointsApiExplorer();

		// builder.Services.AddControllers(options =>
		// {
		// 	options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
		// 	options.UseNamespaceRouteToken();
		//
		// }).AddJsonOptions(options =>
		// {
		// 	options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
		// });

		builder.Services.AddTransient(typeof(IOptionalDependency<>), typeof(OptionalDependency<>));

		// builder.Services.AddApiVersioning(options =>
		// {
		// 	options.DefaultApiVersion = ApiVersionParser.Default.Parse(v1.ApiVersion.Number);
		// 	options.AssumeDefaultVersionWhenUnspecified = true;
		// 	options.ReportApiVersions = true;
		// 	options.ApiVersionReader = ApiVersionReader.Combine(
		// 		new UrlSegmentApiVersionReader()
		// 	);
		// }).AddApiExplorer(options =>
		// {
		// 	options.GroupNameFormat = "'v'VVV";
		// 	options.SubstituteApiVersionInUrl = true;
		// });

		builder.Services.AddSingleton(_ => TimeProvider.System);
		builder.Services.AddBinacleServices();

		builder.Services.AddOpenApiDocumentsFromAssemblyContaining<IApiMarker>();
		// 	.AddOpenApi("v1");
		// builder.Services.AddOpenApi("v2");
		// builder.Services.AddOpenApi("v3", options =>
		// {
		// 	options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0;
		// });
		// builder.Services.AddSwaggerExamples(options =>
		// {
		// 	options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
		// 	// ignore null
		// 	options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
		// });
		//
		// builder.Services.AddSwaggerGen();
		// builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

		// builder.Services.Configure<ApiBehaviorOptions>(options =>
		// {
		// 	options.SuppressModelStateInvalidFilter = true;
		// });

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

		app.MapOpenApi("/openapi/{documentName}.json");
		app.UseSwaggerUI(options =>
		{
			options.SwaggerEndpoint("/openapi/v1.json", "v1");
			options.SwaggerEndpoint("/openapi/v2.json", "v2");
			options.SwaggerEndpoint("/openapi/v3.json", "v3");
		});
		app.MapScalarApiReference(options =>
		{
			options.AddDocument("v1");
			options.AddDocument("v2");
			options.AddDocument("v3");
		});
		// // SWAGGER_UI from environment vars
		// if (Feature.IsEnabled("SWAGGER_UI"))
		// {
		// 	app.UseSwagger();
		// 	app.UseSwaggerUI(options =>
		// 	{
		// 		ConfigureSwaggerOptions.ConfigureSwaggerUI(options, app);
		//
		// 		if (Feature.IsEnabled("SERVICE_MODULE"))
		// 		{
		// 			options.ConfigureServiceModuleSwaggerUI(app);
		// 		}
		// 	});
		// }

		app.UseDiagnosticsModule();

		if (Feature.IsEnabled("SERVICE_MODULE"))
		{
			app.UseServiceModule();
		}

		if (Feature.IsEnabled("UI_MODULE"))
		{
			app.UseUIModule();
		}

		app.RegisterEndpointsFromAssemblyContaining<IApiMarker>();
		await app.RunStartupTasksAsync();
		await app.RunAsync();
	}
}
