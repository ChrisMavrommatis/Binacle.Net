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

internal class Get : IGroupedEndpoint<AdminGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapGet("/account/{id}", HandleAsync)
			.WithSummary("Get account")
			.WithDescription("Admins can use this endpoint to get account information.")
			.Produces(StatusCodes.Status200OK)
			.WithResponseDescription(StatusCodes.Status200OK, GetAccountResponseDescription.For200OK)
			.ResponseExamples<CreateAccountErrorResponseExamples>(
				StatusCodes.Status400BadRequest,
				"application/json"
			)
			.Produces(StatusCodes.Status404NotFound)
			.WithResponseDescription(StatusCodes.Status404NotFound, AccountResponseDescription.For404NotFound)
			.Produces(StatusCodes.Status409Conflict)
			.WithResponseDescription(StatusCodes.Status409Conflict, AccountResponseDescription.For409Conflict);
	}

	internal async Task<IResult> HandleAsync(
		string id,
		IMediator mediator,
		CancellationToken cancellationToken = default)
	{
		if (!Guid.TryParse(id, out var accountId))
		{
			return Results.BadRequest(
				ErrorResponse.IdToGuidParameterError()
			);
		}

		var query = new GetAccountQuery(accountId);
		
		var result = await mediator.QueryAsync(query, cancellationToken);

		return result.Match(
			account => Results.Ok(account),
			notFound => Results.NotFound()
			// error => Results.BadRequest(ErrorResponse.Create(error.Message ?? "Account creation failed"))
		);
	}
}
