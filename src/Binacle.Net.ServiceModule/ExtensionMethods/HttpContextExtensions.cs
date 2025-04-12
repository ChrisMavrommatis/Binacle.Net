using Microsoft.AspNetCore.Http;

namespace Binacle.Net.ServiceModule.ExtensionMethods;

internal static class HttpContextExtensions
{
	public static string? GetClientIp(this HttpContext httpContext)	
	{
		// Check for the X-Forwarded-For header (behind a proxy)
		if (httpContext.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
		{
			return forwardedFor.ToString();
		}

		if (httpContext.Request.Headers.TryGetValue("X-Real-IP", out var realIp))
		{
			return realIp.ToString();
		}

		return httpContext.Connection.RemoteIpAddress?.ToString();
	}
}
