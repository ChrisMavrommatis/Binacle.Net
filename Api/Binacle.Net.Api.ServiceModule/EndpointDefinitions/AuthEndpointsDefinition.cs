﻿using Binacle.Net.Api.ServiceModule.Models;
using Binacle.Net.Api.ServiceModule.Requests;
using Binacle.Net.Api.ServiceModule.Responses;
using Binacle.Net.Api.ServiceModule.Services;
using ChrisMavrommatis.Api.MinimalEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Binacle.Net.Api.ServiceModule.EndpointDefinitions;

internal class AuthEndpointsDefinition : IEndpointDefinition
{
	public void DefineEndpoints(WebApplication app)
	{
		var group = app.MapGroup("/auth")
			.WithTags("Auth");
		

		group.MapPost("/token", Token)
			.DisableRateLimiting()
			.WithSummary("Authenticate to use the service without limits")
			.WithDescription("Use this endpoint if you have the credentials to get a token so you can make the calls without rate limits")
			.Accepts<TokenRequest>("application/json")
			.WithOpenApi();
	}

	[SwaggerResponse(StatusCodes.Status200OK, "When you have valid credentials", typeof(TokenResponse), "application/json")]
	[SwaggerResponse(StatusCodes.Status400BadRequest, "When the request is invalid", typeof(AuthErrorResponse), "application/json")]
	[SwaggerResponse(StatusCodes.Status401Unauthorized, "When the credentials are invalid")]
	internal async Task<IResult> Token(
		IAuthService authService,
		ITokenService tokenService,
		TimeProvider timeProvider,
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

		var authResult = await authService.AuthenticateAsync(new AuthenticationRequest(request.Email, request.Password), cancellationToken);
		if (!authResult.Success)
		{
			var result = authResult.Reason switch
			{
				AuthenticationFailedResultReason.InvalidCredentials => Results.Unauthorized(),
				_ => Results.BadRequest(AuthErrorResponse.Create("Something went wrong, check your request"))

			};

			return result;
		}

		var tokenResult = tokenService.GenerateStatelessToken(new StatelessTokenGenerationRequest(authResult.User!.Email, authResult.User!.Group));
		if (!tokenResult.Success)
		{
			return Results.BadRequest(AuthErrorResponse.Create("Failed to generate token"));

		}
		return Results.Ok(TokenResponse.Create(tokenResult.TokenType!, tokenResult.Token!, tokenResult.ExpiresIn!.Value));
	}
}
