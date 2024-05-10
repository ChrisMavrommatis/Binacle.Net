using Binacle.Net.Api.ServiceModule.Domain.Configuration.Models;
using Binacle.Net.Api.ServiceModule.v0.Requests;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using Xunit;

namespace Binacle.Net.Api.ServiceIntegrationTests;

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

	[Fact]
	public async Task Test_Infrastructure_Configured_Correctly()
	{
		var defaultsOptions = this.sut.Services.GetRequiredService<IOptions<DefaultsOptions>>();
		var defaultAdminUser = defaultsOptions.Value.GetParsedAdminUser();
		var request = new TokenRequest()
		{
			Email = defaultAdminUser.Email,
			Password = defaultAdminUser.Password
		};
		var response = await this.sut.Client.PostAsJsonAsync("/auth/token", request);
		response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
	}
}
