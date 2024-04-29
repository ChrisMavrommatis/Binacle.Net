using Binacle.Net.Api.ServiceModule.ApiModels.Requests;
using FluentAssertions;
using System.Net.Http.Json;
using Xunit;

namespace Binacle.Net.Api.ServiceModule.IntegrationTests;

[Trait("Sanity Tests", "Ensures the tests are configured correctly")]
public class SanityTests : IClassFixture<BinacleApiAsAServiceFactory>
{
	private readonly BinacleApiAsAServiceFactory sut;

	public SanityTests(BinacleApiAsAServiceFactory sut)
	{
		this.sut = sut;
	}

	[Fact]
	public async Task Tests_Work()
	{
		Xunit.Assert.True(true);
	}
	
	[Fact]
	public async Task Sut_Configured_Correctly()
	{
		var request = new TokenRequest()
		{
			Email = "test@test.com",
			Password = "password123"
		};
		var response = await this.sut.Client.PostAsJsonAsync("/auth/token", request);
		response.StatusCode.Should().NotBe(System.Net.HttpStatusCode.NotFound);
	}
}
