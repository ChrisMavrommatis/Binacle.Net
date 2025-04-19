using Binacle.Net.ServiceModule.Application.Accounts.Services;
using Binacle.Net.ServiceModule.Application.Authentication.Services;
using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using Binacle.Net.ServiceModule.Domain.Accounts.Models;
using FluxResults.TypedResults;
using FluxResults.Unions;
using YetAnotherMediator;

namespace Binacle.Net.ServiceModule.Application.Authentication.Messages;

public record CreateAccountCommand(string Email, string Password)
	: ICommand<FluxUnion<Account, Conflict, UnexpectedError>>;

internal class CreateAccountCommandHandler : ICommandHandler<CreateAccountCommand, FluxUnion<Account, Conflict, UnexpectedError>>
{
	private readonly IAccountRepository accountRepository;
	private readonly IPasswordHasher passwordHasher;
	private readonly TimeProvider timeProvider;

	public CreateAccountCommandHandler(
		IAccountRepository accountRepository,
		IPasswordHasher passwordHasher,
		TimeProvider timeProvider
		)
	{
		this.accountRepository = accountRepository;
		this.passwordHasher = passwordHasher;
		this.timeProvider = timeProvider;
	}
	public async ValueTask<FluxUnion<Account, Conflict, UnexpectedError>> HandleAsync(CreateAccountCommand command, CancellationToken cancellationToken)
	{
		var getResult = await this.accountRepository.GetByEmailAsync(command.Email);

		if (getResult.TryGetValue<Account>(out var existingAccount) && existingAccount is not null && !existingAccount.IsDeleted)
		{
			return TypedResult.Conflict;
		}

		var utcNow = this.timeProvider.GetUtcNow();
		var newAccount = new Account(
			command.Email.ToLowerInvariant(),
			AccountRole.User,
			command.Email,
			AccountStatus.Active,
			utcNow
		);
		
		var passwordHash = this.passwordHasher.CreateHash(command.Password);
		newAccount.ChangePassword(passwordHash);
		
		var createResult = await this.accountRepository.CreateAsync(newAccount);

		if (!createResult.TryGetValue<Success>(out var _))
		{
			return TypedResult.Conflict;
		}
		return newAccount;
	}
}
