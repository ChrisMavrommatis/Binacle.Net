using System.Net;
using System.Net.Http.Json;
using Binacle.Net.ServiceModule.Application.Common.Configuration;
using Binacle.Net.ServiceModule.v0.Contracts.Auth;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Binacle.Net.ServiceModule.IntegrationTests.Endpoints.Auth;

[Trait("Endpoint Tests", "Endpoint Integration tests")]
[Collection(BinacleApiAsAServiceCollection.Name)]
public class Token
{
	private readonly BinacleApiAsAServiceFactory sut;

	public Token(BinacleApiAsAServiceFactory sut)
	{
		this.sut = sut;
	}

	private const string routePath = "/api/auth/token";


	[Fact(DisplayName = $"POST {routePath}. With Valid Credentials Returns 200 OK")]
	public async Task Post_WithValidCredentials_Returns_200OK()
	{
		var options = this.sut.Services.GetRequiredService<IOptions<ServiceModuleOptions>>();
		var defaultAdminUser = ServiceModuleOptions.ParseAccountCredentials(options.Value.DefaultAdminAccount);
		var request = new TokenRequest()
		{
			Username = defaultAdminUser.Username,
			Password = defaultAdminUser.Password
		};
		var response = await this.sut.Client.PostAsJsonAsync(routePath, request, this.sut.JsonSerializerOptions);
		response.StatusCode.ShouldBe(HttpStatusCode.OK);
	}

	[Fact(DisplayName = $"POST {routePath}. With Wrong User/Password Returns 401 Unauthorized")]
	public async Task Post_WithWrongUserPassword_Returns_401Unauthorized()
	{
		var request = new TokenRequest()
		{
			Username = "invalid@nonexisting.test",
			Password = "Wr0ngP@ssw0rd"
		};
		var response = await this.sut.Client.PostAsJsonAsync(routePath, request, this.sut.JsonSerializerOptions);
		response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
	}

	[Fact(DisplayName = $"POST {routePath}. With Invalid Credentials Returns 400 BadRequest")]
	public async Task Post_WithInvalidCredentials_Returns_400BadRequest()
	{
		var request1 = new TokenRequest()
		{
			Username = "notvalidemail.test",
			Password = "Wr0ngP@ssw0rd"
		};
		var response1 = await this.sut.Client.PostAsJsonAsync(routePath, request1, this.sut.JsonSerializerOptions);
		response1.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

		var request2 = new TokenRequest()
		{
			Username = "valid@email.test",
			Password = "invpass"
		};
		var response2 = await this.sut.Client.PostAsJsonAsync(routePath, request2, this.sut.JsonSerializerOptions);
		response2.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
	}
}
