using System.Net;
using System.Net.Http.Headers;
using Binacle.Net.ServiceModule.Application.Authentication.Configuration;
using Binacle.Net.ServiceModule.Application.Authentication.Models;
using Binacle.Net.ServiceModule.Infrastructure.Authentication.Services;
using FluxResults.Unions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;

namespace Binacle.Net.ServiceModule.IntegrationTests.Endpoints.Admin;

public abstract partial class AdminEndpointsTestsBase
{
	protected async Task Action_WithoutBearerToken_Returns_401Unauthorized(Func<Task<HttpResponseMessage>> action)
	{
		this.Sut.Client.DefaultRequestHeaders.Authorization = null;
		var response = await action();
		response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
	}

	protected async Task Action_WithExpiredBearerToken_Returns_401Unauthorized(Func<Task<HttpResponseMessage>> action)
	{
		var timeProvider = new FakeTimeProvider();
		timeProvider.SetLocalTimeZone(TimeZoneInfo.Local);
		timeProvider.SetUtcNow(DateTime.UtcNow.AddHours(-10));

		var jwtAuthOptions = this.Sut.Services.GetService<IOptions<JwtAuthOptions>>();
		var tokenService = new TokenService(jwtAuthOptions!, timeProvider);

		
		var result = tokenService.GenerateToken(this.SimulatedAdminAccount, null);
		var token = result.Unwrap<Token>();

		this.Sut.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.TokenValue);

		var response = await action();

		response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
	}

	protected async Task Action_WithWrongIssuerBearerToken_Returns_401Unauthorized(Func<Task<HttpResponseMessage>> action)
	{
		var jwtAuthOptions = this.Sut.Services.GetService<IOptions<JwtAuthOptions>>()!;

		var newJwtAuthOptions = new JwtAuthOptions()
		{
			TokenSecret = jwtAuthOptions.Value.TokenSecret,
			ExpirationInSeconds = jwtAuthOptions.Value.ExpirationInSeconds,
			Audience = jwtAuthOptions.Value.Audience,
			Issuer = "WrongIssuer"
		};

		var tokenService = new TokenService(Options.Create(newJwtAuthOptions), TimeProvider.System);

		var result = tokenService.GenerateToken(this.SimulatedAdminAccount, null);
		var token = result.Unwrap<Token>();

		this.Sut.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.TokenValue);

		var response = await action();

		response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
	}

	protected async Task Action_WithWrongAudienceBearerToken_Returns_401Unauthorized(Func<Task<HttpResponseMessage>> action)
	{
		var jwtAuthOptions = this.Sut.Services.GetService<IOptions<JwtAuthOptions>>()!;

		var newJwtAuthOptions = new JwtAuthOptions()
		{
			TokenSecret = jwtAuthOptions.Value.TokenSecret,
			ExpirationInSeconds = jwtAuthOptions.Value.ExpirationInSeconds,
			Audience = "WrongAudience",
			Issuer = jwtAuthOptions.Value.Issuer
		};

		var tokenService = new TokenService(Options.Create(newJwtAuthOptions), TimeProvider.System);

		var result = tokenService.GenerateToken(this.SimulatedAdminAccount, null);
		var token = result.Unwrap<Token>();

		this.Sut.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.TokenValue);

		var response = await action();

		response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
	}

	protected async Task Action_WithWronglySignedBearerToken_Returns_401Unauthorized(Func<Task<HttpResponseMessage>> action)
	{
		var jwtAuthOptions = this.Sut.Services.GetService<IOptions<JwtAuthOptions>>()!;

		var newJwtAuthOptions = new JwtAuthOptions()
		{
			TokenSecret = "Bad" + jwtAuthOptions.Value.TokenSecret,
			ExpirationInSeconds = jwtAuthOptions.Value.ExpirationInSeconds,
			Audience = jwtAuthOptions.Value.Audience,
			Issuer = jwtAuthOptions.Value.Issuer
		};

		var tokenService = new TokenService(Options.Create(newJwtAuthOptions), TimeProvider.System);

		var result = tokenService.GenerateToken(this.SimulatedAdminAccount, null);
		var token = result.Unwrap<Token>();

		this.Sut.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.TokenValue);

		var response = await action();

		response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
	}

	protected async Task Action_WithoutAdminUserBearerToken_Returns_403Forbidden(Func<Task<HttpResponseMessage>> action)
	{
		await this.AuthenticateAsAsync(this.UserAccountCredentials);

		var response = await action();

		response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
	}
}
