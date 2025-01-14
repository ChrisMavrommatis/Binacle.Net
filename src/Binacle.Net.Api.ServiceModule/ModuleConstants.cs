namespace Binacle.Net.Api.ServiceModule;

internal static class ModuleConstants
{
	internal static string[] RateLimitedPaths = [
		"/api/v1/query",
		"/api/v2/fit",
		"/api/v2/pack",
		"/api/v3/pack"
	];
}
