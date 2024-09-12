using Binacle.Net.Api.ServiceModule.Domain.Users.Entities;
using Binacle.Net.Api.ServiceModule.Domain.Users.Models;
using Binacle.Net.Api.ServiceModule.Extensions;
using Binacle.Net.Api.ServiceModule.Models;
using Binacle.Net.Api.ServiceModule.Services;
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
			.Accepts<v0.Requests.TokenRequest>("application/json")
			.Produces<v0.Responses.TokenResponse>(StatusCodes.Status200OK, "application/json")
			.Produces<v0.Responses.AuthErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
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

	[SwaggerRequestExample(typeof(v0.Requests.TokenRequest), typeof(v0.Requests.Examples.TokenRequestExample))]
	[SwaggerResponseExample(typeof(v0.Responses.TokenResponse), typeof(v0.Responses.Examples.TokenResponseExample), StatusCodes.Status200OK)]
	[SwaggerResponseExample(typeof(v0.Responses.AuthErrorResponse), typeof(v0.Responses.Examples.AuthErrorResponseExample), StatusCodes.Status400BadRequest)]
	internal async Task<IResult> HandleAsync(
		IUserManagerService userManagerService,
		ITokenService tokenService,
		IValidator<v0.Requests.TokenRequest> validator,
		[FromBody] v0.Requests.TokenRequest request,
		CancellationToken cancellationToken = default
		)
	{
		var validationResult = await validator.ValidateAsync(request, cancellationToken);
		if (!validationResult.IsValid)
		{
			return Results.BadRequest(v0.Responses.AuthErrorResponse.Create("Validation Error", validationResult.Errors.Select(x => x.ErrorMessage).ToArray()));
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
			return Results.BadRequest(v0.Responses.AuthErrorResponse.Create("Failed to generate token"));

		}
		return Results.Ok(v0.Responses.TokenResponse.Create(tokenResult.TokenType!, tokenResult.Token!, tokenResult.ExpiresIn!.Value));
	}
}
