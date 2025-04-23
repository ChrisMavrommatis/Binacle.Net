using System.Collections.Concurrent;
using Binacle.Net.ServiceModule.Application.Accounts.Services;
using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using FluxResults.TypedResults;
using FluxResults.Unions;

namespace Binacle.Net.ServiceModule.Infrastructure.Accounts.Services;

internal class InMemoryAccountRepository : IAccountRepository
{
	private static readonly ConcurrentDictionary<Guid, Account> _accounts = new();

	public Task<FluxUnion<Account, NotFound>> GetByIdAsync(Guid id)
	{
		if (_accounts.TryGetValue(id, out var account) && !account.IsDeleted)
		{
			return Task.FromResult<FluxUnion<Account, NotFound>>(account);
		}

		return Task.FromResult<FluxUnion<Account, NotFound>>(TypedResult.NotFound);
	}

	public Task<FluxUnion<Account, NotFound>> GetByUsernameAsync(string username)
	{
		var account = _accounts.Values.FirstOrDefault(x => x.Username == username);
		if (account is not null && !account.IsDeleted)
		{
			return Task.FromResult<FluxUnion<Account, NotFound>>(account);
		}

		return Task.FromResult<FluxUnion<Account, NotFound>>(TypedResult.NotFound);
	}

	public Task<FluxUnion<Success, Conflict>> CreateAsync(Account account)
	{
		var added = _accounts.TryAdd(account.Id, account);

		return Task.FromResult<FluxUnion<Success, Conflict>>(
			added ? TypedResult.Success : TypedResult.Conflict
		);
	}

	public Task<FluxUnion<Success, NotFound>> UpdateAsync(Account account)
	{
		if (_accounts.TryGetValue(account.Id, out var existing) && existing.IsDeleted)
		{
			var updated = _accounts.TryUpdate(account.Id, account, existing);
			if (updated)
			{
				return Task.FromResult<FluxUnion<Success, NotFound>>(TypedResult.Success);
			}
		}

		return Task.FromResult<FluxUnion<Success, NotFound>>(TypedResult.NotFound);
	}

	public Task<FluxUnion<Success, NotFound>> DeleteAsync(Account account)
	{
		var removed = _accounts.TryRemove(account.Id, out _);
		return Task.FromResult<FluxUnion<Success, NotFound>>(
			removed ? TypedResult.Success : TypedResult.NotFound
		);
	}
}
