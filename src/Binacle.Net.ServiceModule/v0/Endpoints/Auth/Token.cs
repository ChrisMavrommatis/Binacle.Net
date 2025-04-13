using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.ServiceModule.Domain.Users.Entities;
using Binacle.Net.ServiceModule.Domain.Users.Models;
using Binacle.Net.ServiceModule.Constants;
using Binacle.Net.ServiceModule.Models;
using Binacle.Net.ServiceModule.Services;
using Binacle.Net.ServiceModule.v0.Requests;
using Binacle.Net.ServiceModule.v0.Requests.Examples;
using Binacle.Net.ServiceModule.v0.Responses;
using Binacle.Net.ServiceModule.v0.Responses.Examples;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using OpenApiExamples;

namespace Binacle.Net.ServiceModule.v0.Endpoints.Auth;

internal class Token : IEndpoint
{
	public void DefineEndpoint(IEndpointRouteBuilder endpoints)
	{
		endpoints.MapPost("/api/auth/token", HandleAsync)
			.WithTags("Auth")
			.WithSummary("Auth Token")
			.WithDescription(
				"Use this endpoint if you have the credentials to get a token and use the service without limits"
			)
			.Accepts<TokenRequest>("application/json")
			.RequestExample<TokenRequestExample>("application/json")
			.Produces<TokenResponse>(StatusCodes.Status200OK, "application/json")
			.ResponseExample<TokenResponseExample>(StatusCodes.Status200OK, "application/json")
			.WithResponseDescription(StatusCodes.Status200OK, ResponseDescription.ForAuthToken200OK)
			.Produces<AuthErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
			.ResponseExamples<AuthErrorResponseExample>(StatusCodes.Status400BadRequest, "application/json")
			.WithResponseDescription(StatusCodes.Status400BadRequest, ResponseDescription.For400BadRequest)
			.Produces(StatusCodes.Status401Unauthorized)
			.WithResponseDescription(
				StatusCodes.Status401Unauthorized,
				ResponseDescription.ForAuthToken401Unauthorized
			)
			.RequireRateLimiting("Auth");
	}

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
			return Results.BadRequest(
				AuthErrorResponse.Create(
					"Validation Error",
					validationResult.Errors.Select(x => x.ErrorMessage).ToArray()
				)
			);
		}

		var result = await userManagerService.AuthenticateAsync(
			new AuthenticateUserRequest(request.Email, request.Password),
			cancellationToken
		);

		if (!result.Is<User>())
		{
			return Results.Unauthorized();
		}

		var user = result.GetValue<User>();

		var tokenResult = tokenService.GenerateStatelessToken(
			new StatelessTokenGenerationRequest(user.Email, user.Group)
		);
		if (!tokenResult.Success)
		{
			return Results.BadRequest(AuthErrorResponse.Create("Failed to generate token"));
		}

		return Results.Ok(
			TokenResponse.Create(tokenResult.TokenType!, tokenResult.Token!, tokenResult.ExpiresIn!.Value)
		);
	}
}
