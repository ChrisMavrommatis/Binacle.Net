using System.Net.Mime;
using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.ServiceModule.Domain.Accounts.Services;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using Binacle.Net.ServiceModule.v0.Resources;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OpenApiExamples.ExtensionMethods;

namespace Binacle.Net.ServiceModule.v0.Endpoints.Admin.Account;

internal class List : IGroupedEndpoint<AdminGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapGet("/accounts", HandleAsync)
			.WithSummary("List accounts")
			.WithDescription("Admins can use this endpoint to list accounts, one page at a time")
			.Produces<PagedResponse<AccountListItem>>(StatusCodes.Status200OK)
			.ResponseDescription(StatusCodes.Status200OK, "The requested page of accounts")
			.ResponseExample<AccountListResponseExample>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)

			.ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
			.ResponseDescription(
				StatusCodes.Status422UnprocessableEntity,
				ResponseDescription.For422UnprocessableContent
			)
			.ResponseExample<PageQueryValidationProblemExample>(
				StatusCodes.Status422UnprocessableEntity,
				MediaTypeNames.Application.ProblemJson
			);
	}

	internal static async Task<IResult> HandleAsync(
		[AsParameters] PageQuery query,
		IValidator<PageQuery> validator,
		IAccountRepository accountRepository,
		CancellationToken cancellationToken = default)
	{
		var validationResult = await validator.ValidateAsync(query, cancellationToken);
		if (!validationResult.IsValid)
		{
			return Results.ValidationProblem(
				validationResult.GetValidationSummary(),
				statusCode: StatusCodes.Status422UnprocessableEntity
			);
		}

		var pageSize = query.PageSizeOrDefault;
		var page = await accountRepository.ListAsync(
			query.Skip,
			pageSize,
			query.AllowDeleted ?? false,
			cancellationToken
		);

		return Results.Ok(
			new PagedResponse<AccountListItem>()
			{
				Total = page.Total,
				Page = query.PageOrDefault,
				PageSize = pageSize,
				TotalPages = (int)Math.Ceiling(page.Total / (double)pageSize),
				Items = page.Items.Select(AccountListItem.From).ToList()
			}
		);
	}
}
