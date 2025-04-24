using Binacle.Net.ServiceModule.Application.Accounts.Services;
using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using FluxResults.TypedResults;
using FluxResults.Unions;
using YetAnotherMediator;

namespace Binacle.Net.ServiceModule.Application.Accounts.UseCases;

public record DeleteAccountCommand(Guid Id)
	: ICommand<FluxUnion<Success, NotFound, UnexpectedError>>;


internal class DeleteAccountCommandHandler :
	ICommandHandler<DeleteAccountCommand, FluxUnion<Success, NotFound, UnexpectedError>>
{
	private readonly IAccountRepository accountRepository;
	private readonly TimeProvider timeProvider;

	public DeleteAccountCommandHandler(
		IAccountRepository accountRepository,
		TimeProvider timeProvider
	)
	{
		this.accountRepository = accountRepository;
		this.timeProvider = timeProvider;
	}

	public async ValueTask<FluxUnion<Success, NotFound, UnexpectedError>> HandleAsync(
		DeleteAccountCommand command,
		CancellationToken cancellationToken
	)
	{
		var accountResult = await this.accountRepository.GetByIdAsync(command.Id);
		if (!accountResult.TryGetValue<Account>(out var account) || account is null)
		{
			return TypedResult.NotFound;
		}

		var now = this.timeProvider.GetUtcNow();
		account.SoftDelete(now);

		var result = await this.accountRepository.UpdateAsync(account);

		if (result.Is<Success>())
		{
			return TypedResult.Success;
		}

		return TypedResult.UnexpectedError;
	}
}
