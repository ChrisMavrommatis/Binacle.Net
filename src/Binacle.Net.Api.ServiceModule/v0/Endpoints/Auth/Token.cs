using Binacle.Net.Api.ServiceModule.Domain.Users.Entities;
using Binacle.Net.Api.ServiceModule.Domain.Users.Models;
using Binacle.Net.Api.ServiceModule.Models;
using Binacle.Net.Api.ServiceModule.Services;
using Binacle.Net.Api.ServiceModule.v0.Requests;
using Binacle.Net.Api.ServiceModule.v0.Requests.Examples;
using Binacle.Net.Api.ServiceModule.v0.Responses;
using ChrisMavrommatis.MinimalEndpointDefinitions;
using ChrisMavrommatis.SwaggerExamples.Attributes;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.Annotations;

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
			// TODO  SwaggerResponse overwrites SwaggerResponseExample
			//.Produces<AuthErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
			.WithOpenApi();
	}
	
	[SwaggerRequestExample(typeof(TokenRequest), typeof(TokenRequestExample))]

	[SwaggerResponse(StatusCodes.Status200OK, "When you have valid credentials", typeof(TokenResponse), "application/json")]

	//[SwaggerResponseExample(typeof(AuthErrorResponse), typeof(AuthErrorResponseExample), StatusCodes.Status400BadRequest)]
	[SwaggerResponse(StatusCodes.Status400BadRequest, "When the request is invalid", typeof(AuthErrorResponse), "application/json")]

	[SwaggerResponse(StatusCodes.Status401Unauthorized, "When the credentials are invalid")]
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
