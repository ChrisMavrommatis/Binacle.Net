using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using Binacle.Net.ServiceModule.Domain.Accounts.Services;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using Binacle.Net.ServiceModule.v0.Resources;
using FluxResults.Unions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OpenApiExamples;

namespace Binacle.Net.ServiceModule.v0.Endpoints.Admin.Accounts;

internal class Delete : IGroupedEndpoint<AdminGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapDelete("/account/{id}", HandleAsync)
			.WithSummary("Delete an account")
			.WithDescription("Admins can use this endpoint to delete an account")
			.Produces(StatusCodes.Status204NoContent)
			.ResponseDescription(StatusCodes.Status204NoContent, DeleteAccountResponseDescription.For204NoContent)
			.ResponseExamples<DeleteAccountErrorResponseExamples>(
				StatusCodes.Status400BadRequest,
				"application/json"
			)
			.Produces(StatusCodes.Status404NotFound)
			.ResponseDescription(StatusCodes.Status404NotFound, AccountResponseDescription.For404NotFound);
	}

	internal async Task<IResult> HandleAsync(
		string id,
		IAccountRepository accountRepository,
		TimeProvider timeProvider,
		CancellationToken cancellationToken = default
	)
	{
		if (!Guid.TryParse(id, out var accountId))
		{
			return Results.BadRequest(
				ErrorResponse.IdToGuidParameterError
			);
		}

		var accountResult = await accountRepository.GetByIdAsync(accountId);
		if (!accountResult.TryGetValue<Account>(out var account) || account is null)
		{
			return Results.NotFound();
		}

		var now = timeProvider.GetUtcNow();
		account.SoftDelete(now);

		var result = await accountRepository.ForceUpdateAsync(account);

		return result.Match(
			success => Results.NoContent(),
			notFound => Results.NotFound()
		);
	}
}
