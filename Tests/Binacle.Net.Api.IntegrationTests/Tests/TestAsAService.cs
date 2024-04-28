using Binacle.Net.Api.ServiceModule.ApiModels.Requests;
using FluentAssertions;
using System.Net.Http.Json;
using Xunit;

namespace Binacle.Net.Api.IntegrationTests;

public class TestAsAService : IClassFixture<BinacleApiAsAServiceFactory>
{
	private readonly BinacleApiAsAServiceFactory sut;

	public TestAsAService(BinacleApiAsAServiceFactory sut)
	{
		this.sut = sut;
	}

	[Fact]
	public async Task Tests_Work()
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
