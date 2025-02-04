using Binacle.Net.Api.ServiceModule.Domain.Configuration.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace Binacle.Net.Api.ServiceModule.IntegrationTests;

[Trait("Endpoint Tests", "Endpoint Integration tests")]
[Collection(BinacleApiAsAServiceCollection.Name)]
public class AuthToken
{
	private readonly BinacleApiAsAServiceFactory sut;

	public AuthToken(BinacleApiAsAServiceFactory sut)
	{
		this.sut = sut;
	}

	private const string routePath = "/api/auth/token";


	[Fact(DisplayName = $"POST {routePath}. With Valid Credentials Returns 200 OK")]
	public async Task Post_WithValidCredentials_Returns_200OK()
	{
		var userOptions = this.sut.Services.GetRequiredService<IOptions<UserOptions>>();
		var defaultAdminUser = userOptions.Value.GetParsedDefaultAdminUser();
		var request = new ServiceModule.v0.Requests.TokenRequest()
		{
			Email = defaultAdminUser.Email,
			Password = defaultAdminUser.Password
		};
		var response = await this.sut.Client.PostAsJsonAsync(routePath, request, this.sut.JsonSerializerOptions);
		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
	}

	[Fact(DisplayName = $"POST {routePath}. With Wrong User/Password Returns 401 Unauthorized")]
	public async Task Post_WithWrongUserPassword_Returns_401Unauthorized()
	{
		var request = new ServiceModule.v0.Requests.TokenRequest()
		{
			Email = "invalid@nonexisting.test",
			Password = "Wr0ngP@ssw0rd"
		};
		var response = await this.sut.Client.PostAsJsonAsync(routePath, request, this.sut.JsonSerializerOptions);
		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Unauthorized);
	}

	[Fact(DisplayName = $"POST {routePath}. With Invalid Credentials Returns 400 BadRequest")]
	public async Task Post_WithInvalidCredentials_Returns_400BadRequest()
	{
		var request1 = new ServiceModule.v0.Requests.TokenRequest()
		{
			Email = "notvalidemail.test",
			Password = "Wr0ngP@ssw0rd"
		};
		var response1 = await this.sut.Client.PostAsJsonAsync(routePath, request1, this.sut.JsonSerializerOptions);
		response1.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);

		var request2 = new ServiceModule.v0.Requests.TokenRequest()
		{
			Email = "valid@email.test",
			Password = "invpass"
		};
		var response2 = await this.sut.Client.PostAsJsonAsync(routePath, request2, this.sut.JsonSerializerOptions);
		response2.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
	}
}
