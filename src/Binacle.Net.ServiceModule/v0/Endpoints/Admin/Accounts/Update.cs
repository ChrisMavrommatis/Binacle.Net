using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using Binacle.Net.ServiceModule.Domain.Accounts.Services;
using Binacle.Net.ServiceModule.Domain.Common.Services;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using Binacle.Net.ServiceModule.v0.Resources;
using FluxResults.Unions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OpenApiExamples;

namespace Binacle.Net.ServiceModule.v0.Endpoints.Admin.Accounts;

internal class Update : IGroupedEndpoint<AdminGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPut("/account/{id}", HandleAsync)
			.WithSummary("Update an account")
			.WithDescription("Admins can use this endpoint to update an account")
			.Accepts<AccountUpdateRequest>("application/json")
			.RequestExample<AccountUpdateRequestExample>("application/json")
			
			.Produces(StatusCodes.Status204NoContent)
			.ResponseDescription(StatusCodes.Status204NoContent, "The account was updated succesfully")

			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ResponseDescription(StatusCodes.Status400BadRequest, ResponseDescription.For400BadRequest)
			.ResponseExamples<Status400ResponseExamples>(
				StatusCodes.Status400BadRequest,
				"application/problem+json"
			)
			
			.Produces(StatusCodes.Status404NotFound)
			.ResponseDescription(StatusCodes.Status404NotFound, AccountResponseDescription.For404NotFound)
			
			.Produces(StatusCodes.Status409Conflict)
			.ResponseDescription(StatusCodes.Status409Conflict, AccountResponseDescription.For409Conflict)
			
			.ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
			.ResponseDescription(
				StatusCodes.Status422UnprocessableEntity,
				ResponseDescription.For422UnprocessableEntity
			)
			.ResponseExample<AccountUpdateValidationProblemExample>(
				StatusCodes.Status422UnprocessableEntity,
				"application/problem+json"
			);
	}

	internal async Task<IResult> HandleAsync(
		[AsParameters] AccountId id,
		AccountBindingResult<AccountUpdateRequest> bindingResult,
		IAccountRepository accountRepository,
		IPasswordService passwordService,
		CancellationToken cancellationToken = default)
	{
		return await bindingResult.ValidateAsync(id, async (request, account) =>
		{
			var usernameResult = await accountRepository.GetByUsernameAsync(request.Username);
			if (usernameResult.TryGetValue<Account>(out var foundAccount) && account.Equals(foundAccount))
			{
				return Results.Conflict();
			}
			account.ChangeUsername(request.Username);
			account.ChangeEmail(request.Email);
			var newPassword = passwordService.Create(request.Password);
			account.ChangePassword(newPassword);
			account.ChangeRole(request.Role!.Value);
			account.ChangeStatus(request.Status!.Value);
			
			var updateResult = await accountRepository.UpdateAsync(account);

			return updateResult.Match(
				success => Results.NoContent(),
				notFound => Results.NotFound()
			);
		});
	}
}
