using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.ServiceModule.Domain.Accounts.Services;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using Binacle.Net.ServiceModule.v0.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OpenApiExamples;

namespace Binacle.Net.ServiceModule.v0.Endpoints.Admin.Accounts;

internal class Get : IGroupedEndpoint<AdminGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapGet("/account/{id}", HandleAsync)
			.WithSummary("Get account")
			.WithDescription("Admins can use this endpoint to get an account's information")
			.Produces<GetAccountResponse>(StatusCodes.Status200OK)
			.WithResponseDescription(StatusCodes.Status200OK, GetAccountResponseDescription.For200OK)
			.ResponseExample<GetAccountResponse.Example>(StatusCodes.Status200OK, "application/json")
			.ResponseExample<GetAccountResponse.ErrorResponseExample>(
				StatusCodes.Status400BadRequest,
				"application/json"
			)
			.Produces(StatusCodes.Status404NotFound)
			.WithResponseDescription(StatusCodes.Status404NotFound, AccountResponseDescription.For404NotFound);

	}

	internal async Task<IResult> HandleAsync(
		string id,
		IAccountRepository accountRepository,
		CancellationToken cancellationToken = default)
	{
		return await AccountRequestValidationExtensions.WithTryCatch(async () =>
		{
			if (!Guid.TryParse(id, out var accountId))
			{
				return Results.BadRequest(
					ErrorResponse.IdToGuidParameterError
				);
			}
			var result = await accountRepository.GetByIdAsync(accountId);

			return result.Match(
				account => Results.Ok(
					GetAccountResponse.From(account)
				),
				notFound => Results.NotFound()
			);
		});
	}
}
