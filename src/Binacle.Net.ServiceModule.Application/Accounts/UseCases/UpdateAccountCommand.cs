using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using Binacle.Net.ServiceModule.Domain.Accounts.Models;
using FluxResults.TypedResults;
using FluxResults.Unions;
using YetAnotherMediator;

namespace Binacle.Net.ServiceModule.Application.Accounts.UseCases;

public record UpdateAccountCommand(
	Guid Id,
	string? Email = null,
	string? Username = null,
	string? Password = null,
	AccountRole? Role = null,
	AccountStatus? Status = null
	) : ICommand<FluxUnion<Account, Conflict, UnexpectedError>>;



internal class UpdateAccountCommandHandler : ICommandHandler<UpdateAccountCommand, FluxUnion<Account, Conflict, UnexpectedError>>
{
	public ValueTask<FluxUnion<Account, Conflict, UnexpectedError>> HandleAsync(UpdateAccountCommand command, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}
}
