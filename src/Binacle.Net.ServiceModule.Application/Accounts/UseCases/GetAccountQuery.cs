using Binacle.Net.ServiceModule.Application.Accounts.Services;
using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using FluxResults.TypedResults;
using FluxResults.Unions;
using YetAnotherMediator;

namespace Binacle.Net.ServiceModule.Application.Accounts.UseCases;

public record GetAccountQuery(
	Guid Id
) : IQuery<FluxUnion<Account, NotFound>>;

internal class GetAccountQueryHandler : IQueryHandler<GetAccountQuery, FluxUnion<Account, NotFound>>
{
	private readonly IAccountRepository accountRepository;

	public GetAccountQueryHandler(
		IAccountRepository accountRepository
	)
	{
		this.accountRepository = accountRepository;
	}
	public async ValueTask<FluxUnion<Account, NotFound>> HandleAsync(GetAccountQuery query, CancellationToken cancellationToken)
	{
		var getResult = await this.accountRepository.GetByIdAsync(query.Id);
		return getResult;
	}
}

