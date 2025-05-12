using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using Binacle.Net.ServiceModule.Domain.Accounts.Services;
using Binacle.Net.ServiceModule.Domain.Common.Models;
using Binacle.Net.ServiceModule.Infrastructure.Common.Models;
using FluxResults.TypedResults;
using FluxResults.Unions;

namespace Binacle.Net.ServiceModule.Infrastructure.Accounts.Services;

internal class InMemoryAccountRepository : IAccountRepository
{
	private static readonly ConcurrentSortedDictionary<Guid, Account> _accounts = new();

	public Task<FluxUnion<Account, NotFound>> GetByIdAsync(Guid id, bool allowDeleted = false, CancellationToken cancellationToken = default)
	{
		if (_accounts.TryGetValue(id, out var account) && (allowDeleted || !account.IsDeleted))
		{
			return Task.FromResult<FluxUnion<Account, NotFound>>(account);
		}

		return Task.FromResult<FluxUnion<Account, NotFound>>(TypedResult.NotFound);
	}

	public Task<PagedList<Account>> ListAsync(int page, int pageSize, CancellationToken cancellationToken = default)
	{
		var accounts = _accounts.Values
			.Where(x => !x.IsDeleted)
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.ToList();

		var pagedAccounts = new PagedList<Account>(
			accounts,
			accounts.Count,
			pageSize,
			page
		);

		return Task.FromResult(pagedAccounts);
	}

	public Task<FluxUnion<Account, NotFound>> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
	{
		var account = _accounts.Values.FirstOrDefault(x => x.Username == username);
		if (account is not null && !account.IsDeleted)
		{
			return Task.FromResult<FluxUnion<Account, NotFound>>(account);
		}

		return Task.FromResult<FluxUnion<Account, NotFound>>(TypedResult.NotFound);
	}

	public Task<FluxUnion<Success, Conflict>> CreateAsync(Account account, CancellationToken cancellationToken = default)
	{
		if (_accounts.ContainsKey(account.Id))
		{
			return Task.FromResult<FluxUnion<Success, Conflict>>(TypedResult.Conflict);
		}

		_accounts.Add(account.Id, account);
		return Task.FromResult<FluxUnion<Success, Conflict>>(TypedResult.Success);
	}

	public Task<FluxUnion<Success, NotFound>> UpdateAsync(Account account, CancellationToken cancellationToken = default)
	{
		if (_accounts.TryGetValue(account.Id, out var existingAccount))
		{
			_accounts[account.Id] = account;
			return Task.FromResult<FluxUnion<Success, NotFound>>(TypedResult.Success);
		}

		return Task.FromResult<FluxUnion<Success, NotFound>>(TypedResult.NotFound);
	}
	
	public Task<FluxUnion<Success, NotFound>> DeleteAsync(Account account, CancellationToken cancellationToken = default)
	{
		var removed = _accounts.Remove(account.Id);
		return Task.FromResult<FluxUnion<Success, NotFound>>(
			removed ? TypedResult.Success : TypedResult.NotFound
		);
	}
}
