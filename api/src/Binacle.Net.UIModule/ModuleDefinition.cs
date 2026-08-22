using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace Binacle.Net.UIModule;

public static class ModuleDefinition
{
	public static void AddUIModule(this WebApplicationBuilder builder)
	{
		Log.Information("{ModuleName} module. Status {Status}", "UI", "Initializing");

		builder.WebHost.UseStaticWebAssets();

		builder.Services.Configure<FeatureOptions>(options =>
		{
			options.AddFeature("UIModule");
		});

		// The seam for pointing the demo at another API host. Nothing sets it - see UIModuleOptions.
		builder.Services.Configure<UIModuleOptions>(options =>
		{
			options.ApiBaseUrl = string.Empty;
		});

		// Static web assets. A missing bundle is a 404, never a page.
		builder.Services.Configure<ReservedPathOptions>(options =>
		{
			options.AddPrefix("/_content");
		});

		builder.Services.AddRazorPages();
		builder.Services.AddSingleton<Services.AppletsService>();

		Log.Information("{ModuleName} module. Status {Status}", "UI", "Initialized");
	}

	public static void UseUIModule(this WebApplication app)
	{
		app.MapStaticAssets();

		// Has to sit on app. Inside a UseWhen branch the re-execute finds no endpoint and returns 0 bytes.
		app.UseStatusCodePagesWithReExecute("/error/{0}");

		// A reserved path answers with whatever the endpoint wrote - a bare status, or problem-details JSON.
		app.Use(async (context, next) =>
		{
			if (IsReserved(context))
			{
				var statusCodeFeature = context.Features.Get<IStatusCodePagesFeature>();

				if (statusCodeFeature is { Enabled: true })
					statusCodeFeature.Enabled = false;
			}

			await next();
		});

		// A bare status with no body, so the re-execute above renders the error page. UseExceptionHandler
		// cannot do this job: when its handler writes nothing it falls back to problem-details JSON.
		app.Use(async (context, next) =>
		{
			try
			{
				await next();
			}
			catch (Exception exception)
			{
				if (IsReserved(context) || context.Response.HasStarted)
					throw;

				Log.Error(exception, "Unhandled exception serving {Path}", context.Request.Path);
				context.Response.Clear();
				context.Response.StatusCode = StatusCodes.Status500InternalServerError;
			}
		});

		app.MapRazorPages()
			.WithStaticAssets();
	}

	// Resolved per request: the reserved set is built lazily, on first read.
	private static bool IsReserved(HttpContext context)
		=> context.RequestServices
			.GetRequiredService<IOptions<ReservedPathOptions>>().Value
			.Covers(context.Request.Path);
}
