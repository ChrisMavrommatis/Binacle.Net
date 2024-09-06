using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Binacle.Net.Api.UIModule.Components;

namespace Binacle.Net.Api.UIModule;

public static class ModuleDefinition
{
	public static void AddUIModule(this WebApplicationBuilder builder)
	{
		Log.Information("{moduleName} module. Status {status}", "UI", "Initializing");

		builder.Services
			.AddRazorComponents()
			.AddInteractiveServerComponents();

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
		app.UseAntiforgery();

		app.MapRazorComponents<App>()
			.AddInteractiveServerRenderMode();

		// handle 404

		app.UseStatusCodePagesWithReExecute("/Error");
		//app.UseStatusCodePagesWithRedirects("/Error");

	}
}

