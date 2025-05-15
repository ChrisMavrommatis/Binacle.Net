using System.Collections.Concurrent;
using Binacle.Net.ServiceModule.Domain.Common.Models;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Entities;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Services;
using Binacle.Net.ServiceModule.Infrastructure.Common.Models;

namespace Binacle.Net.ServiceModule.Infrastructure.Subscriptions.Services;

internal class InMemorySubscriptionRepository : ISubscriptionRepository
{
	private static readonly ConcurrentSortedDictionary<Guid, Subscription> _subscriptions = new();
	
	public Task<FluxUnion<Subscription, NotFound>> GetByIdAsync(Guid id, bool allowDeleted = false, CancellationToken cancellationToken = default)
	{
		if (_subscriptions.TryGetValue(id, out var subscription) && (!subscription.IsDeleted || allowDeleted))
		{
			return Task.FromResult<FluxUnion<Subscription, NotFound>>(subscription);
		}

		return Task.FromResult<FluxUnion<Subscription, NotFound>>(TypedResult.NotFound);
	}
	
	public Task<PagedList<Subscription>> ListAsync(int page, int pageSize, CancellationToken cancellationToken = default)
	{
		var subscriptions = _subscriptions.Values
			.Where(x => !x.IsDeleted)
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.ToList();

		var pagedSubscriptions = new PagedList<Subscription>(
			subscriptions,
			subscriptions.Count,
			pageSize,
			page
		);

		return Task.FromResult(pagedSubscriptions);
	}


	public Task<FluxUnion<Subscription, NotFound>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
	{
		var subscription = _subscriptions.Values.FirstOrDefault(s => s.AccountId == accountId);
		if (subscription is not null && !subscription.IsDeleted)
		{
			return Task.FromResult<FluxUnion<Subscription, NotFound>>(subscription);
		}

		return Task.FromResult<FluxUnion<Subscription, NotFound>>(TypedResult.NotFound);
	}

	public Task<FluxUnion<Success, Conflict>> CreateAsync(Subscription subscription, CancellationToken cancellationToken = default)
	{
		if(_subscriptions.ContainsKey(subscription.Id))
		{
			return Task.FromResult<FluxUnion<Success, Conflict>>(TypedResult.Conflict);
		}
		_subscriptions.Add(subscription.Id, subscription);
		return Task.FromResult<FluxUnion<Success, Conflict>>(TypedResult.Success);
	}

	public Task<FluxUnion<Success, NotFound>> UpdateAsync(Subscription subscription, CancellationToken cancellationToken = default)
	{
		if (_subscriptions.TryGetValue(subscription.Id, out var existing))
		{
			_subscriptions[subscription.Id] = subscription;
			return Task.FromResult<FluxUnion<Success, NotFound>>(TypedResult.Success);
		}

		return Task.FromResult<FluxUnion<Success, NotFound>>(TypedResult.NotFound);
	}
	
	public Task<FluxUnion<Success, NotFound>> DeleteAsync(Subscription subscription, CancellationToken cancellationToken = default)
	{
		var removed = _subscriptions.Remove(subscription.Id);
		return Task.FromResult<FluxUnion<Success, NotFound>>(
			removed ? TypedResult.Success : TypedResult.NotFound
		);
	}
}
