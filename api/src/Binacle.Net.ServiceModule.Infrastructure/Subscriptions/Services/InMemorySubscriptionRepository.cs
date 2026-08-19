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
	
	public Task<FluxUnion<Subscription, NotFound>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
	{
		var subscription = _subscriptions.GetValues().FirstOrDefault(s => s.AccountId == accountId);
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
		if (_subscriptions.TryGetValue(subscription.Id, out _))
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

	public Task<PagedResult<Subscription>> ListAsync(int skip, int take, bool allowDeleted = false, CancellationToken cancellationToken = default)
	{
		// Ordered by the printed Guid, the way the real stores order it. Guid's own comparer is not that order.
		var all = _subscriptions.GetValues()
			.Where(x => allowDeleted || !x.IsDeleted)
			.OrderBy(x => x.Id.ToString(), StringComparer.Ordinal)
			.ToList();

		var items = all.Skip(skip).Take(take).ToList();
		return Task.FromResult(new PagedResult<Subscription>(items, all.Count));
	}
}
