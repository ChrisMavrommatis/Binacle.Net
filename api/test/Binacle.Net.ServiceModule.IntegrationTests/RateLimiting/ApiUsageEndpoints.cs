using System.Text;
using Binacle.Net.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Binacle.Net.ServiceModule.IntegrationTests.RateLimiting;

// Every POST under the v3 and v4 groups is user compute and belongs behind the ApiUsage limiter. The list is
// derived from the routes rather than from the endpoints' own rate limiting metadata: derived from the metadata,
// an endpoint that lost .RequireRateLimiting would drop out of the list instead of failing.
internal static class ApiUsageEndpoints
{
	public static IReadOnlyList<string> RoutesOf(WebApplicationFactory<IApiMarker> api)
	{
		var presets = api.Services.GetRequiredService<IOptions<BinPresetOptions>>().Value;
		var preset = presets.Presets.First(entry => entry.Value.Bins.Count > 0);

		var routes = api.Services.GetRequiredService<EndpointDataSource>().Endpoints
			.OfType<RouteEndpoint>()
			.Where(endpoint => endpoint.Metadata.GetMetadata<HttpMethodMetadata>()?.HttpMethods
				.Contains(HttpMethods.Post) ?? false)
			.Select(endpoint => endpoint.RoutePattern.RawText!)
			.Where(route => route.StartsWith("/api/v3/") || route.StartsWith("/api/v4/"))
			// Route values have to resolve to something real, or the endpoint answers 404 for the preset rather
			// than 400 for the body, and the test stops measuring the limiter.
			.Select(route => route
				.Replace("{preset}", preset.Key)
				.Replace("{bin}", preset.Value.Bins[0].ID))
			.ToList();

		// A derived list that comes back empty reports clean forever.
		routes.ShouldNotBeEmpty();
		return routes;
	}

	// The limiter takes its lease before the endpoint runs, so a body that fails validation still costs a permit.
	// That keeps these tests off the presets, the contracts and the packing algorithms.
	public static StringContent RejectedBody()
		=> new("{}", Encoding.UTF8, "application/json");
}
