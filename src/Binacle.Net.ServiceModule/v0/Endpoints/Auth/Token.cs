using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.ServiceModule.Application.Authentication.Messages;
using Binacle.Net.ServiceModule.v0.Contracts.Auth;
using Binacle.Net.ServiceModule.v0.Resources;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OpenApiExamples;
using YetAnotherMediator;

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
			.WithResponseDescription(StatusCodes.Status200OK, AuthTokenResponseDescription.For200OK)
			.Produces<AuthErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
			.ResponseExamples<AuthErrorResponseExamples>(StatusCodes.Status400BadRequest, "application/json")
			.WithResponseDescription(StatusCodes.Status400BadRequest, ResponseDescription.For400BadRequest)
			.Produces(StatusCodes.Status401Unauthorized)
			.WithResponseDescription(
				StatusCodes.Status401Unauthorized,
				AuthTokenResponseDescription.For200OK
			)
			.Produces(StatusCodes.Status403Forbidden)
			.WithResponseDescription(
				StatusCodes.Status403Forbidden,
				AuthTokenResponseDescription.For403Forbidden
			)
			.RequireRateLimiting("Auth");
	}

	internal async Task<IResult> HandleAsync(
		ValidatedBindingResult<TokenRequest> request,
		IMediator mediator,
		IValidator<TokenRequest> validator,
		CancellationToken cancellationToken = default
	)
	{
		if (request.Value is null)
		{
			return Results.BadRequest(
				AuthErrorResponse.Create(
					"Malformed request",
					["Marlformed request body"]
				)
			);
		}

		if (!request.ValidationResult?.IsValid ?? false)
		{
			return Results.BadRequest(
				AuthErrorResponse.Create(
					"Validation Error",
					request.ValidationResult!.Errors.Select(x => x.ErrorMessage).ToArray()
				)
			);
		}


		var authRequest = new AuthenticationRequest(request.Value.Email, request.Value.Password);
		var result = await mediator.SendAsync(authRequest, cancellationToken);

		return result.Match(
			token => Results.Ok(TokenResponse.Create(token)),
			unauthorized => Results.Unauthorized(),
			error => Results.BadRequest(AuthErrorResponse.Create(error.Message ?? "Failed to generate token"))
		);
	}


}
