using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using Binacle.Net.ServiceModule.Domain.Accounts.Services;
using Binacle.Net.ServiceModule.Domain.Common.Services;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using Binacle.Net.ServiceModule.v0.Resources;
using FluentValidation;
using FluxResults.Unions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OpenApiExamples;

namespace Binacle.Net.ServiceModule.v0.Endpoints.Admin.Account;

internal class Patch : IGroupedEndpoint<AdminGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPatch("/account/{id}", HandleAsync)
			.WithSummary("Partially update an account")
			.WithDescription("Admins can use this endpoint to partially update an account")
			.Accepts<AccountPatchRequest>("application/json")
			.RequestExamples<AccountPatchRequestExamples>("application/json")
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
				ResponseDescription.For422UnprocessableContent
			)
			.ResponseExamples<AccountPatchValidationProblemExamples>(
				StatusCodes.Status422UnprocessableEntity,
				"application/problem+json"
			);
	}

	internal async Task<IResult> HandleAsync(
		[AsParameters] AccountId id,
		AccountBindingResult<AccountPatchRequest> bindingResult,
		IAccountRepository accountRepository,
		IPasswordService passwordService,
		CancellationToken cancellationToken = default)
	{
		return await bindingResult.ValidateAsync(id, async (request, account) =>
		{
			if (!string.IsNullOrWhiteSpace(request.Username))
			{
				var usernameResult = await accountRepository.GetByUsernameAsync(request.Username, cancellationToken);
				if (usernameResult.TryGetValue<Domain.Accounts.Entities.Account>(out var foundAccount) && !account.Equals(foundAccount))
				{
					return Results.Conflict();
				}
				account.ChangeUsername(request.Username);
			}
			if (!string.IsNullOrEmpty(request.Email))
			{
				account.ChangeEmail(request.Email);
			}

			if (!string.IsNullOrEmpty(request.Password))
			{
				var newPassword = passwordService.Create(request.Password);
				account.ChangePassword(newPassword);
			}

			if (request.Role.HasValue)
			{
				account.ChangeRole(request.Role.Value);
			}

			if (request.Status.HasValue)
			{
				account.ChangeStatus(request.Status.Value);
			}

			var updateResult = await accountRepository.UpdateAsync(account, cancellationToken);

			return updateResult.Match(
				success => Results.NoContent(),
				notFound => Results.NotFound()
			);
			
		});
	}
}
