using Binacle.Net.Api.ServiceModule.Domain.Users.Entities;
using Binacle.Net.Api.ServiceModule.Domain.Users.Models;
using Binacle.Net.Api.ServiceModule.Extensions;
using Binacle.Net.Api.ServiceModule.Models;
using Binacle.Net.Api.ServiceModule.Services;
using Binacle.Net.Api.ServiceModule.v0.Requests;
using Binacle.Net.Api.ServiceModule.v0.Requests.Examples;
using Binacle.Net.Api.ServiceModule.v0.Responses;
using Binacle.Net.Api.ServiceModule.v0.Responses.Examples;
using ChrisMavrommatis.MinimalEndpointDefinitions;
using ChrisMavrommatis.SwaggerExamples.Attributes;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Binacle.Net.Api.ServiceModule.v0.Endpoints.Auth;

internal class Token : IEndpointDefinition
{
	public void DefineEndpoint(IEndpointRouteBuilder endpoints)
	{
		endpoints.MapPost("/auth/token", HandleAsync)
			.WithTags("Auth")
			.DisableRateLimiting()
			.WithSummary("Authenticate to use the service without limits")
			.WithDescription("Use this endpoint if you have the credentials to get a token so you can make the calls without rate limits")
			.Accepts<TokenRequest>("application/json")
			.Produces<TokenResponse>(StatusCodes.Status200OK, "application/json")
			.Produces<AuthErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
			.Produces(StatusCodes.Status401Unauthorized)
			.WithOpenApi(operation =>
			{
				operation.SetResponseDescription(StatusCodes.Status200OK, @"**OK** 
				<br /> 
				<p>
					When you have valid credentials.
				</p>");

				operation.SetResponseDescription(StatusCodes.Status400BadRequest, @"**Bad Request** 
				<br /> 
				<p>
					When the request is invalid.
				</p>");

				operation.SetResponseDescription(StatusCodes.Status401Unauthorized, @"**Unauthorized** 
				<br /> 
				<p>
					When the credentials are invalid.
				</p>");

				return operation;
			});
	}

	[SwaggerRequestExample(typeof(TokenRequest), typeof(TokenRequestExample))]
	[SwaggerResponseExample(typeof(TokenResponse), typeof(TokenResponseExample), StatusCodes.Status200OK)]
	[SwaggerResponseExample(typeof(AuthErrorResponse), typeof(AuthErrorResponseExample), StatusCodes.Status400BadRequest)]
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
