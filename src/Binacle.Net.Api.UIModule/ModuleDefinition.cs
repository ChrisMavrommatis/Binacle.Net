using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Binacle.Net.Api.UIModule.Components;
using Microsoft.AspNetCore.Diagnostics;

namespace Binacle.Net.Api.UIModule;

public static class ModuleDefinition
{
	private static string[] disabledPaths = ["/api", "/auth", "/users"];

	public static void AddUIModule(this WebApplicationBuilder builder)
	{
		Log.Information("{moduleName} module. Status {status}", "UI", "Initializing");

		builder.Services
			.AddRazorComponents()
			.AddInteractiveServerComponents();


		builder.Services.AddHttpClient("Self", (serviceProvider, httpClient) =>
		{
			// TODO: Fix this
			var url = builder.Configuration["ASPNETCORE_URLS"];
			httpClient.BaseAddress = new Uri(url);
		});


		// For blazor components this is per connection or tab
		builder.Services.AddScoped<Services.ISampleDataService, Services.SampleDataService>();

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
			if (disabledPaths.Any(p => ctx.Request.Path.Value.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
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

