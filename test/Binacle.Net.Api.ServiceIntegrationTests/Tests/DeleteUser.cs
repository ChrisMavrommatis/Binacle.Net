using Binacle.Net.Api.ServiceIntegrationTests.Models;
using FluentAssertions;
using Xunit;

namespace Binacle.Net.Api.ServiceIntegrationTests;

[Trait("Endpoint Tests", "Endpoint Integration tests")]
[Collection(BinacleApiAsAServiceCollection.Name)]
public class DeleteUser : Abstractions.UsersEndpointTestsBase
{
	private readonly TestUser existingUser;

	public DeleteUser(BinacleApiAsAServiceFactory sut) : base(sut)
	{
		this.existingUser = new TestUser()
		{
			Email = "existing@user.test",
			Password = "Ex1stingUs3rP@ssw0rd"

		};
	}

	private const string routePath = "/users/{email}";

	#region 401 Unauthorized

	[Fact(DisplayName = $"DELETE {routePath}. Without Bearer Token Returns 401 Unauthorized")]
	public Task Delete_WithoutBearerToken_Returns_401Unauthorized()
		=> this.Action_WithoutBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{email}", this.existingUser.Email);
			return await this.Sut.Client.DeleteAsync(url);
		});

	[Fact(DisplayName = $"DELETE {routePath}. With Expired Bearer Token Returns 401 Unauthorized")]
	public Task Delete_WithExpiredBearerToken_Returns_401Unauthorized()
		=> this.Action_WithExpiredBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{email}", this.existingUser.Email);
			return await this.Sut.Client.DeleteAsync(url);
		});


	[Fact(DisplayName = $"DELETE {routePath}. With Wrong Issuer Bearer Token Returns 401 Unauthorized")]
	public Task Delete_WithWrongIssuerBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWrongIssuerBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{email}", this.existingUser.Email);
			return await this.Sut.Client.DeleteAsync(url);
		});

	[Fact(DisplayName = $"DELETE {routePath}. With Wrong Audience Bearer Token Returns 401 Unauthorized")]
	public Task Delete_WithWrongAudienceBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWrongAudienceBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{email}", this.existingUser.Email);
			return await this.Sut.Client.DeleteAsync(url);
		});

	[Fact(DisplayName = $"DELETE {routePath}. With Wrongly Signed Bearer Token Returns 401 Unauthorized")]
	public Task Delete_WithWronglySignedBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWronglySignedBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{email}", this.existingUser.Email);
			return await this.Sut.Client.DeleteAsync(url);
		});

	#endregion

	#region 403 Forbidden

	[Fact(DisplayName = $"DELETE {routePath}. Without Admin User Bearer Token Returns 403 Forbidden")]
	public Task Delete_WithoutAdminUserBearerToken_Returns_403Forbidden()
		=> this.Action_WithoutAdminUserBearerToken_Returns_403Forbidden(async () =>
		{
			var url = routePath.Replace("{email}", this.existingUser.Email);
			return await this.Sut.Client.DeleteAsync(url);
		});
	
	#endregion

	#region 204 No Content

	[Fact(DisplayName = $"DELETE {routePath}. With Valid Credentials Returns 204 No Content")]
	public async Task Delete_WithValidCredentials_Returns_204NoContent()
	{
		await this.AuthenticateAsAsync(this.AdminUser);

		var url = routePath.Replace("{email}", this.existingUser.Email);
		var response = await this.Sut.Client.DeleteAsync(url);
		response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
	}
	#endregion

	#region 400 Bad Request

	[Fact(DisplayName = $"DELETE {routePath}. With Invalid Email Returns 400 BadRequest")]
	public async Task Delete_WithInvalidEmail_Returns_400BadRequest()
	{
		await this.AuthenticateAsAsync(this.AdminUser);

		var url = routePath.Replace("{email}", "existinguser.test");
		var response = await this.Sut.Client.DeleteAsync(url);
		response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
	}
	
	#endregion

	#region 404 Not Found

	[Fact(DisplayName = $"DELETE {routePath}. For Non Existing User Returns 404 Not Found")]
	public async Task Delete_ForNonExistingUser_Returns_404NotFound()
	{
		await this.AuthenticateAsAsync(this.AdminUser);

		var url = routePath.Replace("{email}", "nonexisting@user.test");
		var response = await this.Sut.Client.DeleteAsync(url);
		response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
	}

	#endregion


	public override async Task InitializeAsync()
	{
		await this.CreateUser(this.existingUser);
		await base.InitializeAsync();
	}

	public override async Task DisposeAsync()
	{
		await this.DeleteUser(this.existingUser);
		await base.DisposeAsync();
	}
}
