using System.Net;

namespace Binacle.Net.ServiceModule.IntegrationTests.RateLimiting;

// The other half of the pair. The same endpoints still call .RateLimited() with the module off, and nothing
// reads the marker: no policy, no attribute, no middleware in the pipeline. Without this test, a build that
// limited nobody would still pass the module-on one.
[Trait("Rate Limiting Tests", "Ensures the ApiUsage endpoints are not limited with the ServiceModule off")]
[Collection(nameof(RateLimiterCollection))]
public class ApiUsageServiceModuleOff
{
	[Fact(DisplayName = "No ApiUsage endpoint is limited, however many requests it takes")]
	public async Task No_ApiUsageEndpoint_IsLimited_HoweverManyRequests()
	{
		await using var api = new ServiceModuleOffBinacleApi();
		var client = api.CreateClient();
		var routes = ApiUsageEndpoints.RoutesOf(api);

		// Past what the module-on host would have allowed, so a limiter that survived the flag has to show.
		for (var request = 1; request <= RateLimitedBinacleApi.ApiUsagePermitLimit + 1; request++)
		{
			var response = await client.PostAsync(
				routes[0],
				ApiUsageEndpoints.RejectedBody(),
				TestContext.Current.CancellationToken
			);

			response.StatusCode.ShouldNotBe(HttpStatusCode.NotFound);
			response.StatusCode.ShouldNotBe(HttpStatusCode.TooManyRequests);
		}

		foreach (var route in routes)
		{
			var response = await client.PostAsync(
				route,
				ApiUsageEndpoints.RejectedBody(),
				TestContext.Current.CancellationToken
			);

			response.StatusCode.ShouldNotBe(HttpStatusCode.NotFound, $"POST {route}");
			response.StatusCode.ShouldNotBe(HttpStatusCode.TooManyRequests, $"POST {route}");
		}
	}
}
