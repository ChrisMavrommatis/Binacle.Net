using Binacle.Net.ServiceModule.Application.Accounts.Services;
using Binacle.Net.ServiceModule.Application.Authentication.Services;
using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using Binacle.Net.ServiceModule.Domain.Accounts.Models;
using FluxResults.TypedResults;
using FluxResults.Unions;
using YetAnotherMediator;

namespace Binacle.Net.ServiceModule.Application.Accounts.UseCases;

public record UpdateAccountCommand(
	Guid Id,
	string? Username = null,
	string? Password = null,
	string? Email = null,
	AccountRole? Role = null,
	AccountStatus? Status = null
	) : ICommand<FluxUnion<Success, NotFound, Conflict, UnexpectedError>>;



internal class UpdateAccountCommandHandler : ICommandHandler<UpdateAccountCommand, FluxUnion<Success, NotFound, Conflict, UnexpectedError>>
{
	private readonly IAccountRepository accountRepository;
	private readonly IPasswordHasher passwordHasher;

	public UpdateAccountCommandHandler(
		IAccountRepository accountRepository,
		IPasswordHasher passwordHasher
		)
	{
		this.accountRepository = accountRepository;
		this.passwordHasher = passwordHasher;
	}
	public async ValueTask<FluxUnion<Success, NotFound, Conflict, UnexpectedError>> HandleAsync(UpdateAccountCommand command, CancellationToken cancellationToken)
	{
		var accountResult = await this.accountRepository.GetByIdAsync(command.Id);
		if (!accountResult.TryGetValue<Account>(out var account) || account is null)
		{
			return TypedResult.NotFound;
		}

		if (await this.CheckUniqueUsernameAsync(account, command))
		{
			return TypedResult.Conflict;
		}

		if (!string.IsNullOrEmpty(command.Username))
		{
			account.ChangeUsername(command.Username);
		}
		
		if (!string.IsNullOrEmpty(command.Email))
		{
			account.ChangeEmail(command.Email);
		}
		
		if (!string.IsNullOrEmpty(command.Password))
		{
			var newPasswordHash = this.passwordHasher.CreateHash(command.Password);
			account.ChangePassword(newPasswordHash);
		}

		if (command.Role.HasValue)
		{
			account.ChangeRole(command.Role.Value);
		}
		if (command.Status.HasValue)
		{
			account.ChangeStatus(command.Status.Value);
		}

		var updateResult = await this.accountRepository.UpdateAsync(account);
		
		if (!updateResult.TryGetValue<Success>(out var _))
		{
			return TypedResult.NotFound;
		}
		return TypedResult.Success;



	}

	private async ValueTask<bool> CheckUniqueUsernameAsync(Account account, UpdateAccountCommand command)
	{
		if (string.IsNullOrWhiteSpace(command.Username))
		{
			return false;
		}
		var result = await this.accountRepository.GetByUsernameAsync(command.Username);
		if (!result.TryGetValue<Account>(out var foundAccount))
		{
			return false;
		}

		return account.Equals(foundAccount);
	}

}
