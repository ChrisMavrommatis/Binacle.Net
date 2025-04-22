using System.Net;
using System.Net.Http.Json;
using Binacle.Net.ServiceModule.v0.Contracts.Auth;

namespace Binacle.Net.ServiceModule.IntegrationTests;

[Trait("Sanity Tests", "Ensures the tests are configured correctly")]
[Collection(BinacleApiAsAServiceCollection.Name)]
public class SanityTests
{
	private readonly BinacleApiAsAServiceFactory sut;

	public SanityTests(BinacleApiAsAServiceFactory sut)
	{
		this.sut = sut;
	}

	[Fact]
	public void Tests_Work()
	{
		true.ShouldBe(true);
	}
	
	[Fact]
	public async Task Sut_Configured_Correctly()
	{
		var request = new TokenRequest()
		{
			Username = "test@test.com",
			Password = "password123"
		};
		var response = await this.sut.Client.PostAsJsonAsync("/api/auth/token", request, this.sut.JsonSerializerOptions);
		response.StatusCode.ShouldNotBe(HttpStatusCode.NotFound);
	}
}
