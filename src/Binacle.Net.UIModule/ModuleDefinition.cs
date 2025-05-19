using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Binacle.Net.UIModule.Components;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Binacle.Net.UIModule;

public static class ModuleDefinition
{
	public static void AddUIModule(this WebApplicationBuilder builder)
	{
		Log.Information("{moduleName} module. Status {status}", "UI", "Initializing");

		builder.AddJsonConfiguration(
			filePath: "UiModule/ConnectionStrings.json",
			environmentFilePath: $"UiModule/ConnectionStrings.{builder.Environment.EnvironmentName}.json",
			optional: false,
			reloadOnChange: true
		);
		
		builder.Services
			.AddHttpContextAccessor()
			.AddRazorComponents(options =>
			{
			})
			.AddInteractiveServerComponents(options =>
			{
			});

		builder.Services.AddHttpClient("BinacleApi", (serviceProvider, httpClient) =>
		{
			var configuration = serviceProvider.GetRequiredService<IConfiguration>();
			var connectionString = configuration.GetConnectionStringWithEnvironmentVariableFallback("BinacleApi");

			if (connectionString is not null)
			{
				httpClient.BaseAddress = new Uri(connectionString.Get("endpoint")!);
				return;
			}
			
			var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
			var request = httpContextAccessor.HttpContext?.Request;

			if (request is null) 
				return;
			
			var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
			httpClient.BaseAddress = new Uri(baseUrl);
		});

		builder.Services.AddSingleton<Services.AppletsService>();
		
		// For blazor components this is per connection or tab
		builder.Services.AddScoped<Services.ThemeService>();
		builder.Services.AddScoped<Services.MessagingService>();
		builder.Services.AddScoped<Services.BinacleVisualizerService>();
		builder.Services.AddScoped<Services.LocalStorageService>();
		builder.Services.AddScoped<Services.ISampleDataService, Services.SampleDataService>();

		Log.Information("{moduleName} module. Status {status}", "UI", "Initialized");
	}

	public static void UseUIModule(this WebApplication app)
	{
		// Configure the HTTP request pipeline.
		if (!app.Environment.IsDevelopment())
		{
			// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
			app.UseHsts();
		}
		
		app.UseStaticFiles();

		app.MapRazorComponents<App>()
			.AddInteractiveServerRenderMode();
		
		app.UseStatusCodePagesWithReExecute("/Error/{0}");

		app.Use(async(ctx, next) =>
		{
			if (ctx.Request.Path.StartsWithSegments("/api")
			    || ctx.Request.Path.StartsWithSegments("/swagger")
			    || ctx.Request.Path.StartsWithSegments("/scalar"))
			{
				var statusCodeFeature = ctx.Features.Get<IStatusCodePagesFeature>();

				if (statusCodeFeature is { Enabled: true })
					statusCodeFeature.Enabled = false;
			}

			await next();
		});

		app.UseAntiforgery();
	}
}

