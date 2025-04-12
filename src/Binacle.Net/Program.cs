using System.Text.Json;
using System.Text.Json.Serialization;
using Binacle.Net.Configuration.Models;
using Binacle.Net.DiagnosticsModule;
using Binacle.Net.ExtensionMethods;
using Binacle.Net.Kernel.OpenApi;
using Binacle.Net.Kernel.OpenApi.ExtensionsMethods;
using Binacle.Net.ServiceModule;
using Binacle.Net.Services;
using Binacle.Net.UIModule;
using ChrisMavrommatis.Features;
using ChrisMavrommatis.StartupTasks;
using FluentValidation;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Json;
using OpenApiExamples;
using Scalar.AspNetCore;
using Serilog;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Binacle.Net;

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

		builder.Services.AddValidatorsFromAssemblyContaining<IApiMarker>(
			ServiceLifetime.Singleton,
			includeInternalTypes: true
		);
		builder.Services.AddEndpointsApiExplorer();


		builder.Services.ConfigureHttpJsonOptions(options =>
		{
			options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
		});
		builder.Services.Configure<JsonOptions>(options =>
		{
			options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
		});

		builder.Services.AddTransient(typeof(IOptionalDependency<>), typeof(OptionalDependency<>));

		builder.Services.AddSingleton(_ => TimeProvider.System);
		builder.Services.AddBinacleServices();

		builder.Services.AddOpenApiDocumentsFromAssemblyContaining<IApiMarker>();

		builder.Services.AddOpenApiExamples(options =>
		{
			options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
			options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
		});

		builder.Services.Configure<RouteOptions>(options =>
		{
			options.LowercaseQueryStrings = true;
			options.LowercaseUrls = true;
		});


		// builder.Services.AddProblemDetails(options =>
		// {
		// 	options.CustomizeProblemDetails = context =>
		// 	{
		// 		context.ProblemDetails.Instance =
		// 			$"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
		// 		context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
		// 		var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
		// 		context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
		// 	};
		// });
		
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
		
		// app.UseExceptionHandler(exceptionHandlerApp =>
		// {
		// 	exceptionHandlerApp.Run(async httpContext =>
		// 	{
		// 		await Results.Problem()
		// 			.ExecuteAsync(httpContext);
		// 	});
		// });

		// SWAGGER_UI from environment vars
		if (Feature.IsEnabled("SWAGGER_UI"))
		{
			const string openApiEndpointPattern = "/openapi/{documentName}.json";
			app.MapOpenApi(openApiEndpointPattern);

			var openApiDocuments = app.Services.GetServices<IOpenApiDocument>();

			app.UseSwaggerUI(options =>
			{
				foreach (var openApiDocument in openApiDocuments)
				{
					var endpoint = openApiEndpointPattern.Replace("{documentName}", openApiDocument.Name);
					options.SwaggerEndpoint(endpoint, openApiDocument.Title);
				}
			});

			app.MapScalarApiReference(options =>
			{
				foreach (var openApiDocument in openApiDocuments)
				{
					options.AddDocument(openApiDocument.Name, openApiDocument.Title);
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

		app.RegisterEndpointsFromAssemblyContaining<IApiMarker>();

		await app.RunStartupTasksAsync();
		await app.RunAsync();
	}
}
