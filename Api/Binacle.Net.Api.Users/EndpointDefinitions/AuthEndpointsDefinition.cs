using Binacle.Net.Api.Users.Data.Schemas;
using Binacle.Net.Api.Users.Data.Services;
using Binacle.Net.Api.Users.Models;
using Binacle.Net.Api.Users.Requests;
using Binacle.Net.Api.Users.Responses;
using Binacle.Net.Api.Users.Services;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Binacle.Net.Api.Users.EndpointDefinitions;

internal class AuthEndpointsDefinition : IEndpointDefinition
{
	public void DefineEndpoints(WebApplication app)
	{
		var group = app.MapGroup("/auth")
			.WithTags("Auth");

		group.MapPost("/token", Token)
			.DisableRateLimiting();

		group.MapPost("/create", Create)
			.RequireAuthorization(builder =>
			{
				builder.RequireAuthenticatedUser();
				builder.RequireClaim(JwtApplicationClaimNames.Groups, UserGroups.Admins);
			});
	}

	[SwaggerOperation("Authenticate to use the service without limits")]
	[SwaggerResponse(StatusCodes.Status200OK, "The token was generated successfully", typeof(TokenResponse))]
	[SwaggerResponse(StatusCodes.Status400BadRequest, "The request was invalid", typeof(AuthErrorResponse))]
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

	internal async Task<IResult> Create(
		IAuthService authService,
		IValidator<CreateApiUserRequest> validator,
		[FromBody] CreateApiUserRequest request,
		CancellationToken cancellationToken = default)
	{
		var validationResult = await validator.ValidateAsync(request, cancellationToken);
		if (!validationResult.IsValid)
		{
			return Results.BadRequest(AuthErrorResponse.Create("Validation Error", validationResult.Errors.Select(x => x.ErrorMessage).ToArray()));
		}

		var createUserResult = await authService.CreateAsync(new CreateUserRequest(request.Email, request.Password), cancellationToken);
		if (!createUserResult.Success)
		{
			var result = createUserResult.Reason switch
			{
				CreateUserFailedResultReason.InvalidCredentials => Results.BadRequest(AuthErrorResponse.Create("Invalid credentials")),
				CreateUserFailedResultReason.AlreadyExists => Results.Conflict(AuthErrorResponse.Create("User already exists")),
				_ => Results.BadRequest(AuthErrorResponse.Create("Something went wrong, check your request"))
			};
			return result;

		}
		return Results.Created();
	}
}
