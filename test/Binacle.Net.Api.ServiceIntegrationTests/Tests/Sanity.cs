using Binacle.Net.Api.ServiceModule.v0.Requests;
using FluentAssertions;
using System.Net.Http.Json;
using Xunit;

namespace Binacle.Net.Api.ServiceIntegrationTests.Tests;

[Trait("Sanity Tests", "Ensures the tests are configured correctly")]
[Collection(BinacleApiAsAServiceCollection.Name)]
public class Sanity
{
	private readonly BinacleApiAsAServiceFactory sut;

	public Sanity(BinacleApiAsAServiceFactory sut)
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
		var response = await this.sut.Client.PostAsJsonAsync("/auth/token", request, this.sut.JsonSerializerOptions);
		response.StatusCode.Should().NotBe(System.Net.HttpStatusCode.NotFound);
	}
}
