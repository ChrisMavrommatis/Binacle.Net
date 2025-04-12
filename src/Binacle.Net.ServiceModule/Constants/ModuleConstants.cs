namespace Binacle.Net.ServiceModule.Constants;

internal static class RateLimiter
{
	internal static string[] Paths = [
		"/api/v1/query",
		"/api/v2/fit",
		"/api/v2/pack",
		"/api/v3/pack"
	];
}
