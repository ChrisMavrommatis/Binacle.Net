using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using Binacle.Net.ServiceModule.Domain.Accounts.Models;
using Binacle.Net.ServiceModule.Domain.Accounts.Services;
using Binacle.Net.ServiceModule.Domain.Common.Services;
using Binacle.Net.ServiceModule.Services;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;
using Binacle.Net.ServiceModule.v0.Resources;
using FluxResults.Unions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OpenApiExamples;

namespace Binacle.Net.ServiceModule.v0.Endpoints.Admin.Accounts;

internal class Create : IGroupedEndpoint<AdminGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPost("/account", HandleAsync)
			.WithSummary("Create account")
			.WithDescription("Admins can use this endpoint to create accounts")
			.Accepts<CreateAccountRequest>("application/json")
			.RequestExample<CreateAccountRequest.Example>("application/json")
			.Produces(StatusCodes.Status201Created)
			.WithResponseDescription(StatusCodes.Status201Created, CreateAccountResponseDescription.For201Created)
			// TODO
			.ProducesValidationProblem()
			// .ResponseExamples<CreateAccountRequest.ErrorResponseExamples>(
			// 	StatusCodes.Status400BadRequest,
			// 	"application/json"
			// )
			.Produces(StatusCodes.Status409Conflict)
			.WithResponseDescription(StatusCodes.Status409Conflict, AccountResponseDescription.For409Conflict);
	}

	internal async Task<IResult> HandleAsync(
		BindingResult<CreateAccountRequest> bindingResult,
		IAccountRepository accountRepository,
		IPasswordService passwordService,
		TimeProvider timeProvider,
		CancellationToken cancellationToken = default)
	{
		 return await bindingResult.ValidateAsync(async request =>
		 {
			var getResult = await accountRepository.GetByUsernameAsync(request.Username);
			if (getResult.Is<Account>())
			{
				return Results.Conflict();
			}
			
			var utcNow = timeProvider.GetUtcNow();
			var newAccount = new Account(
				request.Username,
				AccountRole.User,
				request.Email.ToLowerInvariant(),
				AccountStatus.Active,
				utcNow
			);
			
			var password = passwordService.Create(request.Password);
			newAccount.ChangePassword(password);
			var createResult = await accountRepository.CreateAsync(newAccount);

			return createResult.Match(
				success => Results.Created($"/api/admin/account/{newAccount.Id}", null),
				conflict => Results.Conflict()
			);
		 });
	}
}
