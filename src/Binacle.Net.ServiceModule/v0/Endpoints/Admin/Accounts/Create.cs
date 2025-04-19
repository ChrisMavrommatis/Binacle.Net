using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.ServiceModule.Application.Authentication.Messages;
using Binacle.Net.ServiceModule.Constants;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;
using Binacle.Net.ServiceModule.v0.Contracts.Auth;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
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
using YetAnotherMediator;

namespace Binacle.Net.ServiceModule.v0.Endpoints.Admin.Accounts;

internal class Create : IGroupedEndpoint<AdminGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPost("/account", HandleAsync)
			.WithSummary("Create an account")
			.WithDescription("Admins use this endpoint to create accounts");
		// .Accepts<CreateApiUserRequest>("application/json")
		// .RequestExample<CreateApiUserRequestExample>("application/json")
		// .Produces(StatusCodes.Status201Created)
		// .WithResponseDescription(StatusCodes.Status201Created, ResponseDescription.ForCreate200OK)
		// .ResponseExamples<CreateApiUserErrorResponseExample>(
		// 	StatusCodes.Status400BadRequest,
		// 	"application/json"
		// )
		// .Produces(StatusCodes.Status409Conflict)
		// .WithResponseDescription(StatusCodes.Status409Conflict, ResponseDescription.ForCreate409Conflict);
	}

	internal async Task<IResult> HandleAsync(
		IMediator mediator,
		ValidatedBindingResult<CreateAccountRequest> request,
		CancellationToken cancellationToken = default)
	{
		if (request.Value is null)
		{
			return Results.BadRequest(
				ErrorResponse.Create(
					"Malformed request",
					["Marlformed request body"]
				)
			);
		}

		if (!request.ValidationResult?.IsValid ?? false)
		{
			return Results.BadRequest(
				ErrorResponse.Create(
					"Validation Error",
					request.ValidationResult!.Errors.Select(x => x.ErrorMessage).ToArray()
				)
			);
		}
		
		var createAccountCommand = new CreateAccountCommand(request.Value.Email, request.Value.Password);
		var result = await mediator.ExecuteAsync(createAccountCommand, cancellationToken); 

		return result.Match(
			account => Results.Created(),
			conflict => Results.Conflict(),
			error => Results.BadRequest(ErrorResponse.Create(error.Message ?? "Account creation failed"))
		);
	}
}
