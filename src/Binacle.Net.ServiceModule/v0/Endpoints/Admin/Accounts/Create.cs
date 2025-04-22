using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.ServiceModule.Application.Accounts.UseCases;
using Binacle.Net.ServiceModule.Domain.Accounts.Models;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using Binacle.Net.ServiceModule.v0.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OpenApiExamples;
using YetAnotherMediator;

namespace Binacle.Net.ServiceModule.v0.Endpoints.Admin.Accounts;

internal class Create : IGroupedEndpoint<AdminGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPost("/account", HandleAsync)
			.WithSummary("Create account")
			.WithDescription("Admins can use this endpoint to create accounts")
			.Accepts<CreateAccountRequest>("application/json")
			.RequestExample<CreateAccountRequestExample>("application/json")
			.Produces(StatusCodes.Status201Created)
			.WithResponseDescription(StatusCodes.Status201Created, CreateAccountResponseDescription.For201Created)
			.ResponseExamples<CreateAccountErrorResponseExamples>(
				StatusCodes.Status400BadRequest,
				"application/json"
			)
			.Produces(StatusCodes.Status409Conflict)
			.WithResponseDescription(StatusCodes.Status409Conflict, AccountResponseDescription.For409Conflict);
	}

	internal async Task<IResult> HandleAsync(
		IMediator mediator,
		ValidatedBindingResult<CreateAccountRequest> request,
		CancellationToken cancellationToken = default)
	{
		if (request.Value is null)
		{
			return Results.BadRequest(
				ErrorResponse.MalformedRequest()
			);
		}

		if (!request.ValidationResult?.IsValid ?? false)
		{
			return Results.BadRequest(
				ErrorResponse.ValidationError(
					request.ValidationResult!.Errors.Select(x => x.ErrorMessage).ToArray()
				)
			);
		}

		var createAccountCommand = new CreateAccountCommand(
			request.Value.Username,
			request.Value.Password,
			request.Value.Email,
			AccountRole.User
		); 
		
		var result = await mediator.ExecuteAsync(createAccountCommand, cancellationToken);

		return result.Match(
			account => Results.Created($"/api/admin/account/{account.Id}", null),
			conflict => Results.Conflict(),
			error => Results.BadRequest(ErrorResponse.Create(error.Message ?? "Account creation failed"))
		);
	}
}
