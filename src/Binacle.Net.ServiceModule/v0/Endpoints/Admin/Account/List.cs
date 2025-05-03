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

namespace Binacle.Net.ServiceModule.v0.Endpoints.Admin.Account;

internal class List : IGroupedEndpoint<AdminGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapGet("/account/", HandleAsync)
			.WithSummary("List accounts")
			.WithDescription("Admins can use this endpoint to list all accounts.")
			.Produces<AccountListResponse>(StatusCodes.Status200OK)
			.ResponseDescription(StatusCodes.Status200OK, "Lists the accounts with pagination")
			.ResponseExample<AccountListResponseExample>(StatusCodes.Status200OK, "application/json")
			
			.ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
			.ResponseDescription(
				StatusCodes.Status422UnprocessableEntity,
				ResponseDescription.For422UnprocessableContent
			)
			.ResponseExample<AccountListValidationProblemExample>(
				StatusCodes.Status422UnprocessableEntity,
				"application/problem+json"
			);
	}

	internal async Task<IResult> HandleAsync(
		[AsParameters] PagingQuery paging,
		IValidator<PagingQuery> validator,
		IAccountRepository accountRepository,
		CancellationToken cancellationToken = default)
	{
		var validationResult = await validator.ValidateAsync(paging, cancellationToken);
		if (!validationResult.IsValid)
		{
			return Results.ValidationProblem(
				validationResult!.GetValidationSummary(),
				statusCode: StatusCodes.Status422UnprocessableEntity
			);
		}
	
		var result = await accountRepository.ListAsync(paging.PageNumber, paging.PageSize);

		return Results.Ok(
			AccountListResponse.Create(result)
		);
	}
}
