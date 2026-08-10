using System.Text.Json;
using System.Text.Json.Serialization;
using Binacle.Net.Configuration;
using Binacle.Net.DiagnosticsModule;
using Binacle.Net.ExtensionMethods;
using Binacle.Net.Kernel.Features;
using Binacle.Net.Kernel.OpenApi;
using Binacle.Net.Kernel.OpenApi.ExtensionsMethods;
using Binacle.Net.ServiceModule;
using Binacle.Net.Services;
using Binacle.Net.UIModule;
using FluentValidation;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Json;
using OpenApiExamples.ExtensionMethods;
using Scalar.AspNetCore;
using Serilog;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Binacle.Net;

public static class Program
{
	public static async Task Main(string[] args)
	{
		var builder = WebApplication.CreateSlimBuilder(args);
		builder.BootstrapLogger();

		// CreateSlimBuilder omits both of these; the default builder wires them up for you.
		builder.WebHost.UseKestrelHttpsConfiguration();
		builder.WebHost.UseQuic(); // HTTP/3 support

		builder.Configuration
			.SetBasePath($"{Directory.GetCurrentDirectory()}/Config_Files");

		builder.Configuration.AddEnvironmentVariables();

		builder.AddJsonConfiguration(
			filePath: "appsettings.json",
			environmentFilePath: $"appsettings.{builder.Environment.EnvironmentName}.json",
			optional: true,
			reloadOnChange: true
		);

		Log.Information("{ModuleName} module. Status {Status}", "Core", "Initializing");

		builder.AddValidatableJsonConfigurationOptions<BinPresetOptions>();
		builder.AddValidatableJsonConfigurationOptions<CorsOptions>();
		builder.AddValidatableJsonConfigurationOptions<ForwardedHeadersConfigurationOptions>();

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
			options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
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


		builder.Services.AddExceptionHandler<InternalServerErrorExceptionHandler>();
		builder.Services.AddProblemDetails(options =>
		{
			options.CustomizeProblemDetails = context =>
			{
				context.ProblemDetails.Instance =
					$"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
				context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
				var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
				context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
			};
		});

		var corsOptions = builder.Configuration
			.GetSection(CorsOptions.SectionName)
			.Get<CorsOptions>();


		builder.Services.AddCors(options =>
		{
			options.AddPolicy(CorsPolicy.CoreApi, policy =>
			{
				policy.WithOrigins(corsOptions?.CoreApi?.AllowedOrigins ?? [])
					.AllowAnyHeader()
					.AllowAnyMethod();
			});
		});


		builder.ConfigureForwardedHeaders();

		Log.Information("{ModuleName} module. Status {Status}", "Core", "Initialized");

		builder.AddDiagnosticsModule();

		if (Feature.IsEnabled("SERVICE_MODULE"))
		{
			builder.AddServiceModule();
		}

		if (Feature.IsEnabled("UI_MODULE"))
		{
			builder.AddUIModule();
		}

		var swaggerEnabled = Feature.IsEnabled("SWAGGER_UI");
		var scalarEnabled = Feature.IsEnabled("SCALAR_UI");

		builder.Services.Configure<FeatureOptions>(options =>
		{
			if (swaggerEnabled)
			{
				options.AddFeature("SwaggerUI");
			}

			if (scalarEnabled)
			{
				options.AddFeature("ScalarUI");
			}
		});

		var app = builder.Build();

		// Rewrites Connection.RemoteIpAddress and Request.Scheme from the proxy's values to the caller's, so it has
		// to run before anything reads either one. Takes its options from DI; the overload that accepts an instance
		// would bypass them.
		app.UseForwardedHeaders();

		// Also omitted by CreateSlimBuilder.
		app.UseHttpsRedirection();

		app.UseExceptionHandler();

		app.UseCors();

		app.MapApiDocumentUi(swaggerEnabled, scalarEnabled);

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
