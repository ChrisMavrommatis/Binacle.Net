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

internal class List : IGroupedEndpoint<AdminGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapGet("/account/", HandleAsync)
			.WithSummary("List accounts")
			.WithDescription("Admins can use this endpoint to list all accounts.")
			.Produces<ListAccountsResponse>(StatusCodes.Status200OK)
			.WithResponseDescription(StatusCodes.Status200OK, ListAccountResponseDescription.For200OK)
			.ResponseExample<ListAccountsResponse.Example>(StatusCodes.Status200OK, "application/json")
			.ResponseExamples<ListAccountsResponse.ErrorResponseExamples>(
				StatusCodes.Status400BadRequest,
				"application/json"
			)
			.Produces(StatusCodes.Status404NotFound)
			.WithResponseDescription(StatusCodes.Status404NotFound, ListAccountResponseDescription.For404NotFound);
	}

	internal async Task<IResult> HandleAsync(
		int? pg,
		int? pz,
		IAccountRepository accountRepository,
		CancellationToken cancellationToken = default)
	{
		return await AccountRequestValidationExtensions.WithTryCatch(async () =>
		{
			if (pg is < 1)
			{
				return Results.BadRequest(
					ErrorResponse.PageNumberError
				);
			}
		
			if (pz is < 1)
			{
				return Results.BadRequest(
					ErrorResponse.PageSizeError
				);
			}
			var result = await accountRepository.GetAsync(pg ?? 1, pz ?? 10);

			return result.Match(
				accounts => Results.Ok(
					ListAccountsResponse.From(accounts)
				),
				notFound => Results.NotFound()
			);
		});
	}
}
