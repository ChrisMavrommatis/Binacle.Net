using Microsoft.AspNetCore.Builder;
using Serilog;

namespace Binacle.Net.Api.ServiceModule;

public static class ModuleDefinition
{
	public static void AddUIModule(this WebApplicationBuilder builder)
	{
		Log.Information("{moduleName} module. Status {status}", "UI", "Initializing");

		Log.Information("{moduleName} module. Status {status}", "UI", "Initialized");
	}

	public static void UseUIModule(this WebApplication app)
	{
		// Middleware are in order
		app.UseStaticFiles(); // Serve static files (the SPA)
							  // Fallback route to serve SPA
		app.MapFallbackToFile("index.html"); // Handle all non-API routes by serving index.html
	}


}
