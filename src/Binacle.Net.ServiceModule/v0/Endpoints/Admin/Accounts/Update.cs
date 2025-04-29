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
			.Accepts<UpdateAccountRequest>("application/json")
			.RequestExample<UpdateAccountRequest.Example>("application/json")
			.Produces(StatusCodes.Status204NoContent)
			.WithResponseDescription(StatusCodes.Status204NoContent, UpdateAccountResponseDescription.For204NoContent)
			.ResponseExamples<UpdateAccountRequest.ErrorResponseExamples>(
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
		AccountBindingResult<UpdateAccountRequest> bindingResult,
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
