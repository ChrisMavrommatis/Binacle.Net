using System.Net.Http.Json;

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
		var request = new v0.Requests.TokenRequest()
		{
			Email = "test@test.com",
			Password = "password123"
		};
		var response = await this.sut.Client.PostAsJsonAsync("/api/auth/token", request, this.sut.JsonSerializerOptions);
		response.StatusCode.ShouldNotBe(System.Net.HttpStatusCode.NotFound);
	}
}
