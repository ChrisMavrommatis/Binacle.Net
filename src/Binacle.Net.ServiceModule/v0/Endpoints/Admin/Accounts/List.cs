using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.ServiceModule.Domain.Accounts.Services;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using Binacle.Net.ServiceModule.v0.Resources;
using FluentValidation;
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
			.ResponseDescription(StatusCodes.Status200OK, ListAccountResponseDescription.For200OK)
			.ResponseExample<ListAccountsResponse.Example>(StatusCodes.Status200OK, "application/json")
			.Produces(StatusCodes.Status400BadRequest)
			.ResponseExamples<ListAccountsResponse.ErrorResponseExamples>(
				StatusCodes.Status400BadRequest,
				"application/json"
			)
			.Produces(StatusCodes.Status404NotFound)
			.ResponseDescription(StatusCodes.Status404NotFound, ListAccountResponseDescription.For404NotFound);
	}

	internal async Task<IResult> HandleAsync(
		[AsParameters] PagingQuery paging,
		IValidator<PagingQuery> validator,
		IAccountRepository accountRepository,
		CancellationToken cancellationToken = default)
	{
		var validationResult =await validator.ValidateAsync(paging, cancellationToken);
		if (!validationResult.IsValid)
		{
			return Results.ValidationProblem(
				validationResult!.GetValidationSummary()
			);
		}
	

		var result = await accountRepository.ListAsync(paging.PageNumber, paging.PageSize);

		return result.Match(
			accounts => Results.Ok(
				ListAccountsResponse.From(accounts)
			),
			notFound => Results.NotFound()
		);
	}
}
