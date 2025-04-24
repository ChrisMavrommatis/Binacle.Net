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

internal class Delete : IGroupedEndpoint<AdminGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapDelete("/account/{id}", HandleAsync)
			.WithSummary("Delete an account")
			.WithDescription("Admins can use this endpoint to delete an account")
			.Produces(StatusCodes.Status204NoContent)
			.WithResponseDescription(StatusCodes.Status204NoContent, DeleteAccountResponseDescription.For204NoContent)
			.ResponseExamples<DeleteAccountErrorResponseExamples>(
				StatusCodes.Status400BadRequest,
				"application/json"
			)
			.Produces(StatusCodes.Status404NotFound)
			.WithResponseDescription(StatusCodes.Status404NotFound, AccountResponseDescription.For404NotFound);
	}

	internal async Task<IResult> HandleAsync(
			string id,
			IMediator mediator,
			CancellationToken cancellationToken = default
		)
	{
		if (!Guid.TryParse(id, out var accountId))
		{
			return Results.BadRequest(
				ErrorResponse.IdToGuidParameterError
			);
		}
		var command = new DeleteAccountCommand(accountId);

		var result = await mediator.ExecuteAsync(command, cancellationToken);	

		return result.Match(
			ok => Results.NoContent(),
			notFound => Results.NotFound(),
			error => Results.BadRequest(ErrorResponse.Create(error.Message ?? "Account deletion failed"))
		);
	}
}
