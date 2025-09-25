using System.Net;
using System.Net.Http.Json;
using Binacle.Net.ServiceModule.v0.Contracts.Auth;

namespace Binacle.Net.ServiceModule.IntegrationTests;

[Trait("Sanity Tests", "Ensures the tests are configured correctly")]
public class SanityTests
{
	private readonly BinacleApi sut;

	public SanityTests(BinacleApi sut)
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
			Username = "test@test.binacle.net",
			Password = "password123"
		};
		var response = await this.sut.Client.PostAsJsonAsync(
			"/api/auth/token",
			request, 
			this.sut.JsonSerializerOptions, 
			TestContext.Current.CancellationToken
		);
		
		response.StatusCode.ShouldNotBe(HttpStatusCode.NotFound);
	}
}
