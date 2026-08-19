using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using Binacle.Net.ServiceModule.Domain.Accounts.Services;
using Binacle.Net.ServiceModule.Domain.Common.Models;
using Binacle.Net.ServiceModule.Infrastructure.Common.Models;

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

	public Task<FluxUnion<Account, NotFound>> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
	{
		var account = _accounts.GetValues().FirstOrDefault(x => x.Username == username);
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
		if (_accounts.TryGetValue(account.Id, out _))
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

	public Task<PagedResult<Account>> ListAsync(int skip, int take, bool allowDeleted = false, CancellationToken cancellationToken = default)
	{
		// Ordered by the printed Guid, the way the real stores order it. Guid's own comparer is not that order.
		var all = _accounts.GetValues()
			.Where(x => allowDeleted || !x.IsDeleted)
			.OrderBy(x => x.Id.ToString(), StringComparer.Ordinal)
			.ToList();

		var items = all.Skip(skip).Take(take).ToList();
		return Task.FromResult(new PagedResult<Account>(items, all.Count));
	}
}
