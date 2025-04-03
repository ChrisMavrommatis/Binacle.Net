using Binacle.Net.Api.Kernel.Endpoints;
using Binacle.Net.Api.ServiceModule.Constants;
using Binacle.Net.Api.ServiceModule.Domain.Users.Entities;
using Binacle.Net.Api.ServiceModule.Domain.Users.Models;
using Binacle.Net.Api.ServiceModule.Models;
using Binacle.Net.Api.ServiceModule.Services;
using Binacle.Net.Api.ServiceModule.v0.Requests;
using Binacle.Net.Api.ServiceModule.v0.Responses;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Binacle.Net.Api.ServiceModule.v0.Endpoints.Auth;

internal class Token : IEndpoint
{
	public void DefineEndpoint(IEndpointRouteBuilder endpoints)
	{
		endpoints.MapPost("/api/auth/token", HandleAsync)
			.WithTags("Auth")
			.DisableRateLimiting()
			.WithSummary("Auth Token")
			.WithDescription("Use this endpoint if you have the credentials to get a token and use the service without limits")
			.Accepts<TokenRequest>("application/json")
			.Produces<TokenResponse>(StatusCodes.Status200OK, "application/json")
			.WithResponseDescription(StatusCodes.Status200OK, ResponseDescription.ForAuthToken200OK)
			.Produces<AuthErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
			.WithResponseDescription(StatusCodes.Status400BadRequest, ResponseDescription.For400BadRequest)
			.Produces(StatusCodes.Status401Unauthorized)
			.WithResponseDescription(StatusCodes.Status401Unauthorized, ResponseDescription.ForAuthToken401Unauthorized);
	}

	// [SwaggerRequestExample(typeof(v0.Requests.TokenRequest), typeof(v0.Requests.Examples.TokenRequestExample))]
	// [SwaggerResponseExample(typeof(v0.Responses.TokenResponse), typeof(v0.Responses.Examples.TokenResponseExample), StatusCodes.Status200OK)]
	// [SwaggerResponseExample(typeof(v0.Responses.AuthErrorResponse), typeof(v0.Responses.Examples.AuthErrorResponseExample), StatusCodes.Status400BadRequest)]
	internal async Task<IResult> HandleAsync(
		IUserManagerService userManagerService,
		ITokenService tokenService,
		IValidator<TokenRequest> validator,
		[FromBody] TokenRequest request,
		CancellationToken cancellationToken = default
		)
	{
		var validationResult = await validator.ValidateAsync(request, cancellationToken);
		if (!validationResult.IsValid)
		{
			return Results.BadRequest(AuthErrorResponse.Create("Validation Error", validationResult.Errors.Select(x => x.ErrorMessage).ToArray()));
		}

		var result = await userManagerService.AuthenticateAsync(new AuthenticateUserRequest(request.Email, request.Password), cancellationToken);

		if (!result.Is<User>())
		{
			return Results.Unauthorized();
		}
		var user = result.GetValue<User>();

		var tokenResult = tokenService.GenerateStatelessToken(new StatelessTokenGenerationRequest(user.Email, user.Group));
		if (!tokenResult.Success)
		{
			return Results.BadRequest(AuthErrorResponse.Create("Failed to generate token"));

		}
		return Results.Ok(TokenResponse.Create(tokenResult.TokenType!, tokenResult.Token!, tokenResult.ExpiresIn!.Value));
	}
}
