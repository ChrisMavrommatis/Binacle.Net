using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using Binacle.Net.ServiceModule.Domain.Accounts.Models;
using Binacle.Net.ServiceModule.Domain.Accounts.Services;
using Binacle.Net.ServiceModule.Domain.Common.Services;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using Binacle.Net.ServiceModule.v0.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OpenApiExamples.ExtensionMethods;

namespace Binacle.Net.ServiceModule.v0.Endpoints.Admin.Account;

internal class Create : IGroupedEndpoint<AdminGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPost("/account", HandleAsync)
			.WithSummary("Create account")
			.WithDescription("Admins can use this endpoint to create accounts")
			.Accepts<AccountCreateRequest>("application/json")
			.RequestExample<AccountCreateRequestExample>("application/json")
			
			.Produces(StatusCodes.Status201Created)
			.ResponseDescription(StatusCodes.Status201Created, "The account was created succesfully.")
			
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ResponseDescription(StatusCodes.Status400BadRequest, ResponseDescription.For400BadRequest)
			.ResponseExamples<Status400ResponseExamples>(
				StatusCodes.Status400BadRequest,
				"application/problem+json"
			)
			
			.Produces(StatusCodes.Status409Conflict)
			.ResponseDescription(StatusCodes.Status409Conflict, AccountResponseDescription.For409Conflict)
			
			.ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
			.ResponseDescription(
				StatusCodes.Status422UnprocessableEntity,
				ResponseDescription.For422UnprocessableContent
			)
			.ResponseExample<AccountCreateValidationProblemExample>(
				StatusCodes.Status422UnprocessableEntity,
				"application/problem+json"
			);
	}

	internal async Task<IResult> HandleAsync(
		BindingResult<AccountCreateRequest> bindingResult,
		IAccountRepository accountRepository,
		IPasswordService passwordService,
		TimeProvider timeProvider,
		CancellationToken cancellationToken = default)
	{
		 return await bindingResult.ValidateAsync(async request =>
		 {
			var getResult = await accountRepository.GetByUsernameAsync(request.Username, cancellationToken);
			if (getResult.Is<Domain.Accounts.Entities.Account>())
			{
				return Results.Conflict();
			}
			
			var utcNow = timeProvider.GetUtcNow();
			var newAccount = new Domain.Accounts.Entities.Account(
				request.Username,
				AccountRole.User,
				request.Email.ToLowerInvariant(),
				AccountStatus.Active,
				utcNow
			);
			
			var password = passwordService.Create(request.Password);
			newAccount.ChangePassword(password);
			var createResult = await accountRepository.CreateAsync(newAccount, cancellationToken);

			return createResult.Match(
				success => Results.Created($"/api/admin/account/{newAccount.Id}", null),
				conflict => Results.Conflict()
			);
		 });
	}
}
