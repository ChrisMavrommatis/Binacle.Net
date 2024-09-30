using Binacle.Net.Api.ServiceModule.IntegrationTests.Models;
using FluentAssertions;
using System.Net.Http.Json;
using Xunit;

namespace Binacle.Net.Api.ServiceModule.IntegrationTests;

[Trait("Endpoint Tests", "Endpoint Integration tests")]
[Collection(BinacleApiAsAServiceCollection.Name)]
public class UpdateUser : Abstractions.UsersEndpointTestsBase
{
	private readonly TestUser existingUser;

	public UpdateUser(BinacleApiAsAServiceFactory sut) : base(sut)
	{
		this.existingUser = new TestUser()
		{
			Email = "existing@user.test",
			Password = "Ex1stingUs3rP@ssw0rd"

		};
	}

	private const string routePath = "/api/users/{email}";

	#region 401 Unauthorized

	[Fact(DisplayName = $"PUT {routePath}. Without Bearer Token Returns 401 Unauthorized")]
	public Task Put_WithoutBearerToken_Returns_401Unauthorized()
		=> this.Action_WithoutBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{email}", this.existingUser.Email);
			var request = new ServiceModule.v0.Requests.UpdateApiUserRequest
			{
				Status = ServiceModule.Models.UserStatus.Active,
				Type = ServiceModule.Models.UserType.Admin
			};
			return await this.Sut.Client.PutAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		});

	[Fact(DisplayName = $"PUT {routePath}. With Expired Bearer Token Returns 401 Unauthorized")]
	public Task Put_WithExpiredBearerToken_Returns_401Unauthorized()
		=> this.Action_WithExpiredBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{email}", this.existingUser.Email);
			var request = new ServiceModule.v0.Requests.UpdateApiUserRequest
			{
				Status = ServiceModule.Models.UserStatus.Active,
				Type = ServiceModule.Models.UserType.Admin
			};
			return await this.Sut.Client.PutAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		});


	[Fact(DisplayName = $"PUT {routePath}. With Wrong Issuer Bearer Token Returns 401 Unauthorized")]
	public Task Put_WithWrongIssuerBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWrongIssuerBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{email}", this.existingUser.Email);
			var request = new ServiceModule.v0.Requests.UpdateApiUserRequest
			{
				Status = ServiceModule.Models.UserStatus.Active,
				Type = ServiceModule.Models.UserType.Admin
			};
			return await this.Sut.Client.PutAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		});

	[Fact(DisplayName = $"PUT {routePath}. With Wrong Audience Bearer Token Returns 401 Unauthorized")]
	public Task Put_WithWrongAudienceBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWrongAudienceBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{email}", this.existingUser.Email);
			var request = new ServiceModule.v0.Requests.UpdateApiUserRequest
			{
				Status = ServiceModule.Models.UserStatus.Active,
				Type = ServiceModule.Models.UserType.Admin
			};
			return await this.Sut.Client.PutAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		});

	[Fact(DisplayName = $"PUT {routePath}. With Wrongly Signed Bearer Token Returns 401 Unauthorized")]
	public Task Put_WithWronglySignedBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWronglySignedBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{email}", this.existingUser.Email);
			var request = new ServiceModule.v0.Requests.UpdateApiUserRequest
			{
				Status = ServiceModule.Models.UserStatus.Active,
				Type = ServiceModule.Models.UserType.Admin
			};
			return await this.Sut.Client.PutAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		});

	#endregion

	#region 403 Forbidden

	[Fact(DisplayName = $"PUT {routePath}. Without Admin User Bearer Token Returns 403 Forbidden")]
	public Task Put_WithoutAdminUserBearerToken_Returns_403Forbidden()
		=> this.Action_WithoutAdminUserBearerToken_Returns_403Forbidden(async () =>
		{
			var url = routePath.Replace("{email}", this.existingUser.Email);
			var request = new ServiceModule.v0.Requests.UpdateApiUserRequest
			{
				Status = ServiceModule.Models.UserStatus.Active,
				Type = ServiceModule.Models.UserType.Admin
			};
			return await this.Sut.Client.PutAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		});

	#endregion

	#region 204 No Content

	[Fact(DisplayName = $"PUT {routePath}. With Both Status And Type Returns 204 No Content")]
	public async Task Put_WithBothStatusAndType_Returns_204NoContent()
	{
		await this.AuthenticateAsAsync(this.AdminUser);

		var url = routePath.Replace("{email}", this.existingUser.Email);
		var request = new ServiceModule.v0.Requests.UpdateApiUserRequest
		{
			Status = ServiceModule.Models.UserStatus.Active,
			Type = ServiceModule.Models.UserType.Admin
		};
		var response = await this.Sut.Client.PutAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
	}

	[Fact(DisplayName = $"PUT {routePath}. With Just Status Returns 204 No Content")]
	public async Task Put_WithJustStatus_Returns_204NoContent()
	{
		await this.AuthenticateAsAsync(this.AdminUser);

		var url = routePath.Replace("{email}", this.existingUser.Email);
		var request = new ServiceModule.v0.Requests.UpdateApiUserRequest
		{
			Status = ServiceModule.Models.UserStatus.Active,
		};
		var response = await this.Sut.Client.PutAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
	}

	[Fact(DisplayName = $"PUT {routePath}. With Just Type Returns 204 No Content")]
	public async Task Put_WithJustType_Returns_204NoContent()
	{
		await this.AuthenticateAsAsync(this.AdminUser);

		var url = routePath.Replace("{email}", this.existingUser.Email);
		var request = new ServiceModule.v0.Requests.UpdateApiUserRequest
		{
			Type = ServiceModule.Models.UserType.Admin
		};
		var response = await this.Sut.Client.PutAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
	}
	#endregion

	#region 400 Bad Request

	[Fact(DisplayName = $"PUT {routePath}. With Invalid Email Returns 400 BadRequest")]
	public async Task Put_WithInvalidEmail_Returns_400BadRequest()
	{
		await this.AuthenticateAsAsync(this.AdminUser);

		var url = routePath.Replace("{email}", "existinguser.test");

		var request = new ServiceModule.v0.Requests.UpdateApiUserRequest
		{
			Status = ServiceModule.Models.UserStatus.Active,
			Type = ServiceModule.Models.UserType.Admin
		};
		var response = await this.Sut.Client.PutAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
	}

	[Fact(DisplayName = $"PUT {routePath}. With Invalid Status Returns 400 BadRequest")]
	public async Task Put_WithInvalidStatus_Returns_400BadRequest()
	{
		await this.AuthenticateAsAsync(this.AdminUser);

		var url = routePath.Replace("{email}", this.existingUser.Email);

		var request = new
		{
			status = "Invalid",
		};
		var response = await this.Sut.Client.PutAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
	}

	[Fact(DisplayName = $"PUT {routePath}. With Invalid Type Returns 400 BadRequest")]
	public async Task Put_WithInvalidType_Returns_400BadRequest()
	{
		await this.AuthenticateAsAsync(this.AdminUser);

		var url = routePath.Replace("{email}", this.existingUser.Email);

		var request = new
		{
			type = "Invalid"
		};
		var response = await this.Sut.Client.PutAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
	}

	#endregion

	#region 404 Not Found

	[Fact(DisplayName = $"PUT {routePath}. For Non Existing User Returns 404 Not Found")]
	public async Task Put_ForNonExistingUser_Returns_404NotFound()
	{
		await this.AuthenticateAsAsync(this.AdminUser);

		var url = routePath.Replace("{email}", "nonexisting@user.test");
		var request = new ServiceModule.v0.Requests.UpdateApiUserRequest
		{
			Status = ServiceModule.Models.UserStatus.Active,
			Type = ServiceModule.Models.UserType.Admin
		};
		var response = await this.Sut.Client.PutAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
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
