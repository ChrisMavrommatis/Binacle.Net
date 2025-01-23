using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Binacle.Net.Api.UIModule.Components;
using Binacle.Net.Api.Kernel;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace Binacle.Net.Api.UIModule;

public static class ModuleDefinition
{
	public static void AddUIModule(this WebApplicationBuilder builder)
	{
		Log.Information("{moduleName} module. Status {status}", "UI", "Initializing");


		builder.Configuration
			.AddJsonFile("UiModule/ConnectionStrings.json", optional: false, reloadOnChange: true)
			.AddJsonFile($"UiModule/ConnectionStrings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
			.AddEnvironmentVariables();

		builder.Services
			.AddHttpContextAccessor()
			.AddRazorComponents()
			.AddInteractiveServerComponents();

		builder.Services.AddHttpClient("BinacleApi", (serviceProvider, httpClient) =>
		{
			var configuration = serviceProvider.GetRequiredService<IConfiguration>();
			var connectionString = configuration.GetConnectionStringWithEnvironmentVariableFallback("BinacleApi", "BINACLEAPI_CONNECTION_STRING");
			
			httpClient.BaseAddress = new Uri(connectionString!.Get("endpoint")!);
		});

		builder.Services.AddScoped<Components.Services.ThemeService>();
		builder.Services.AddSingleton<Components.Services.AppletsService>();

		// For blazor components this is per connection or tab
		builder.Services.AddScoped<Services.ISampleDataService, Services.SampleDataService>();
		// builder.Services.AddScoped<Services.PackingDemoState>();

		Log.Information("{moduleName} module. Status {status}", "UI", "Initialized");
	}

	public static void UseUIModule(this WebApplication app)
	{

		// Configure the HTTP request pipeline.
		if (!app.Environment.IsDevelopment())
		{
			app.UseExceptionHandler("/Error");
			// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
			app.UseHsts();
		}
		
		app.UseStaticFiles();

		app.MapRazorComponents<App>()
			.AddInteractiveServerRenderMode();

		app.UseStatusCodePagesWithReExecute("/Error/{0}");

		app.Use(async(ctx, next) =>
		{
			if (ctx.Request.Path.Value?.StartsWith("/api/", StringComparison.OrdinalIgnoreCase) ?? false)
			{
				var statusCodeFeature = ctx.Features.Get<IStatusCodePagesFeature>();

				if (statusCodeFeature != null && statusCodeFeature.Enabled)
					statusCodeFeature.Enabled = false;
			}

			await next();
		});

		app.UseAntiforgery();
	}
}

