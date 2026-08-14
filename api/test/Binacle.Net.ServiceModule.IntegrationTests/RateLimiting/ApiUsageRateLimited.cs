using System.Net;

namespace Binacle.Net.ServiceModule.IntegrationTests.RateLimiting;

// The ApiUsage policy and the middleware are both registered by the ServiceModule, so with the module on every
// v3 and v4 POST must go through the limiter. Each test builds its own host: the bucket is one partition for all
// anonymous callers and does not refill inside a run, so a shared host would make the tests order dependent.
[Trait("Rate Limiting Tests", "Ensures the ApiUsage endpoints are limited with the ServiceModule on")]
[Collection(nameof(RateLimiterCollection))]
public class ApiUsageRateLimited
{
	[Fact(DisplayName = "Every ApiUsage endpoint answers below the limit")]
	public async Task Every_ApiUsageEndpoint_Answers_BelowTheLimit()
	{
		await using var api = new RateLimitedBinacleApi();
		var client = api.CreateClient();
		var routes = ApiUsageEndpoints.RoutesOf(api);

		routes.Count.ShouldBeLessThan(RateLimitedBinacleApi.ApiUsagePermitLimit);

		foreach (var route in routes)
		{
			var response = await client.PostAsync(
				route,
				ApiUsageEndpoints.RejectedBody(),
				TestContext.Current.CancellationToken
			);

			// A 404 would mean the route moved and the request never reached the pipeline the limiter sits in.
			response.StatusCode.ShouldNotBe(HttpStatusCode.NotFound, $"POST {route}");
			response.StatusCode.ShouldNotBe(HttpStatusCode.TooManyRequests, $"POST {route}");
		}
	}

	[Fact(DisplayName = "Every ApiUsage endpoint returns 429 TooManyRequests once the limit is spent")]
	public async Task Every_ApiUsageEndpoint_Returns_429TooManyRequests_OnceTheLimitIsSpent()
	{
		await using var api = new RateLimitedBinacleApi();
		var client = api.CreateClient();
		var routes = ApiUsageEndpoints.RoutesOf(api);

		// One bucket for every anonymous caller, so spending it on one route spends it for all of them. An
		// endpoint that is not behind the limiter answers 400 here instead of 429.
		for (var request = 1; request <= RateLimitedBinacleApi.ApiUsagePermitLimit; request++)
		{
			var spending = await client.PostAsync(
				routes[0],
				ApiUsageEndpoints.RejectedBody(),
				TestContext.Current.CancellationToken
			);

			spending.StatusCode.ShouldNotBe(HttpStatusCode.TooManyRequests);
		}

		foreach (var route in routes)
		{
			var response = await client.PostAsync(
				route,
				ApiUsageEndpoints.RejectedBody(),
				TestContext.Current.CancellationToken
			);

			response.StatusCode.ShouldBe(HttpStatusCode.TooManyRequests, $"POST {route}");
		}
	}
}
