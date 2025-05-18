using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
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

internal class Delete : IGroupedEndpoint<AdminGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapDelete("/account/{id}", HandleAsync)
			.WithSummary("Delete an account")
			.WithDescription("Admins can use this endpoint to delete an account")
			.Produces(StatusCodes.Status204NoContent)
			.ResponseDescription(StatusCodes.Status204NoContent, "The account was deleted")
			
			.Produces(StatusCodes.Status404NotFound)
			.ResponseDescription(StatusCodes.Status404NotFound, AccountResponseDescription.For404NotFound)
			
			.ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
			.ResponseDescription(
				StatusCodes.Status422UnprocessableEntity,
				ResponseDescription.For422UnprocessableContent
			)
			.ResponseExample<AccountDeleteValidationProblemExample>(
				StatusCodes.Status422UnprocessableEntity,
				"application/problem+json"
			);
	}

	internal async Task<IResult> HandleAsync(
		[AsParameters] AccountId id,
		IValidator<AccountId> validator,
		IAccountRepository accountRepository,
		TimeProvider timeProvider,
		CancellationToken cancellationToken = default
	)
	{
		var validationResult = await validator.ValidateAsync(id, cancellationToken);
		if (!validationResult.IsValid)
		{
			return Results.ValidationProblem(
				validationResult!.GetValidationSummary(),
				statusCode: StatusCodes.Status422UnprocessableEntity
			);
		}

		var accountResult = await accountRepository.GetByIdAsync(id.Value, cancellationToken: cancellationToken);
		if (!accountResult.TryGetValue<Domain.Accounts.Entities.Account>(out var account) || account is null)
		{
			return Results.NotFound();
		}

		var now = timeProvider.GetUtcNow();
		account.SoftDelete(now);

		var result = await accountRepository.UpdateAsync(account, cancellationToken);

		return result.Match(
			success => Results.NoContent(),
			notFound => Results.NotFound()
		);
	}
}
