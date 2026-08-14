using System.Net;
using System.Net.Http.Json;
using Binacle.Net.ServiceModule.v0.Contracts.Auth;

namespace Binacle.Net.ServiceModule.IntegrationTests.RateLimiting;

// The login throttle partitions on Connection.RemoteIpAddress. It once read X-Forwarded-For, which handed any
// caller a fresh limit per attempt. The policy's partition keys are covered by the unit tests; what is here is
// the wiring - that a real request reaches that policy through a live limiter.
[Trait("Rate Limiting Tests", "Ensures the login throttle cannot be reset by the caller")]
[Collection(nameof(RateLimiterCollection))]
public class AuthTokenRateLimited
{
	private const string routePath = "/api/auth/token";

	[Fact(DisplayName = $"POST {routePath}. A forged forwarded header does not reset the limit")]
	public async Task Post_WithAForgedForwardedHeader_Does_Not_Reset_TheLimit()
	{
		await using var api = new RateLimitedBinacleApi();
		var client = api.CreateClient();

		for (var attempt = 1; attempt <= RateLimitedBinacleApi.AuthTokenPermitLimit; attempt++)
		{
			var allowed = await Post(client, attempt);

			allowed.StatusCode.ShouldNotBe(HttpStatusCode.NotFound);
			allowed.StatusCode.ShouldNotBe(HttpStatusCode.TooManyRequests);
		}

		var response = await Post(client, RateLimitedBinacleApi.AuthTokenPermitLimit + 1);

		response.StatusCode.ShouldBe(HttpStatusCode.TooManyRequests);
	}

	// Every attempt claims a different address. Under TestServer there is no connection address at all, so they
	// all share the "unknown" partition - which is the assertion: the header buys nothing.
	private static async Task<HttpResponseMessage> Post(HttpClient client, int attempt)
	{
		var request = new HttpRequestMessage(HttpMethod.Post, routePath)
		{
			Content = JsonContent.Create(new TokenRequest
			{
				Username = "validemail@test.binacle.net",
				Password = "Ag00dP@ssw0rd"
			})
		};
		request.Headers.Add("X-Forwarded-For", $"10.9.9.{attempt}");

		return await client.SendAsync(request, TestContext.Current.CancellationToken);
	}
}
