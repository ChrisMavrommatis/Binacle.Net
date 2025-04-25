using System.Net;
using System.Net.Http.Json;
using Binacle.Net.ServiceModule.Domain;
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
		var defaultAdmin = ServiceModuleOptions.ParseAccountCredentials(options.Value.DefaultAdminAccount);
		var request = new TokenRequest()
		{
			Username = defaultAdmin.Username,
			Password = defaultAdmin.Password
		};
		var response = await this.sut.Client.PostAsJsonAsync(routePath, request, this.sut.JsonSerializerOptions);
		response.StatusCode.ShouldBe(HttpStatusCode.OK);
	}

	[Fact(DisplayName = $"POST {routePath}. With Wrong Credentials Returns 401 Unauthorized")]
	public async Task Post_WithWrongUserPassword_Returns_401Unauthorized()
	{
		var request = new TokenRequest()
		{
			Username = "validemail@binacle.net",
			Password = "Ag00dP@ssw0rd"
		};
		var response = await this.sut.Client.PostAsJsonAsync(routePath, request, this.sut.JsonSerializerOptions);
		response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
	}

	[Fact(DisplayName = $"POST {routePath}. With Invalid Credentials Returns 400 BadRequest")]
	public async Task Post_WithInvalidCredentials_Returns_400BadRequest()
	{
		var request = new TokenRequest()
		{
			Username = "validemail@binacle.net",
			Password = "pass"
		};
		var response = await this.sut.Client.PostAsJsonAsync(routePath, request, this.sut.JsonSerializerOptions);
		response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
	}
}
