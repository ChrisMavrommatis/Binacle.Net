using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.ServiceModule.Application.Accounts.Services;
using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using FluxResults.TypedResults;
using FluxResults.Unions;
using YetAnotherMediator;

namespace Binacle.Net.ServiceModule.Application.Accounts.UseCases;

public record ListAccountsQuery(int Page = 1, int PageSize = 10)
	: IQuery<FluxUnion<PagedList<Account>, NotFound>>;

internal class ListAccountsQueryHandler : IQueryHandler<ListAccountsQuery, FluxUnion<PagedList<Account>, NotFound>>
{
	private readonly IAccountRepository accountRepository;

	public ListAccountsQueryHandler(
		IAccountRepository accountRepository
		)
	{
		this.accountRepository = accountRepository;
	}
	public async ValueTask<FluxUnion<PagedList<Account>, NotFound>> HandleAsync(ListAccountsQuery query, CancellationToken cancellationToken)
	{
		var result = await this.accountRepository.GetAsync(query.Page, query.PageSize);
		return result;
	}
}
