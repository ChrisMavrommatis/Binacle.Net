using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.ServiceModule.Application.Accounts.UseCases;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using Binacle.Net.ServiceModule.v0.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OpenApiExamples;
using YetAnotherMediator;

namespace Binacle.Net.ServiceModule.v0.Endpoints.Admin.Accounts;

internal class Update : IGroupedEndpoint<AdminGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPut("/account/{id}", HandleAsync)
			.WithSummary("Update an account")
			.WithDescription("Admins can use this endpoint to update the account.")
			.Accepts<UpdateAccountRequest>("application/json")
			.RequestExamples<UpdateAccountRequestExamples>("application/json")
			.Produces(StatusCodes.Status204NoContent)
			.WithResponseDescription(StatusCodes.Status204NoContent, UpdateAccountResponseDescription.For204NoContent)
			.ResponseExamples<UpdateAccountErrorResponseExamples>(
				StatusCodes.Status400BadRequest,
				"application/json"
			)
			.Produces(StatusCodes.Status404NotFound)
			.WithResponseDescription(StatusCodes.Status404NotFound, AccountResponseDescription.For404NotFound)
			.Produces(StatusCodes.Status409Conflict)
			.WithResponseDescription(StatusCodes.Status409Conflict, AccountResponseDescription.For409Conflict);
	}

	internal async Task<IResult> HandleAsync(
		string id, // TODO validate it
		ValidatedBindingResult<UpdateAccountRequest> request,
		IMediator mediator,
		CancellationToken cancellationToken = default
	)
	{
		if (request.Value is null)
		{
			return Results.BadRequest(
				ErrorResponse.MalformedRequest()
			);
		}

		if (!Guid.TryParse(id, out var accountId))
		{
			return Results.BadRequest(
				ErrorResponse.IdToGuidParameterError()
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

		var command = new UpdateAccountCommand(
			accountId,
			request.Value.Username,
			request.Value.Password,
			request.Value.Email,
			request.Value.Role,
			request.Value.Status
		);

		var result = await mediator.ExecuteAsync(command);	

		return result.Match(
			ok => Results.NoContent(),
			notFound => Results.NotFound(),
			conglict => Results.Conflict(),
			error => Results.BadRequest(ErrorResponse.Create(error.Message ?? "Account update failed"))
		);
	}
}
