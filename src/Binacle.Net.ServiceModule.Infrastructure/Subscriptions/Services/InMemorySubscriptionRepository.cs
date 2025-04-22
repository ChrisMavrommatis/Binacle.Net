using System.Collections.Concurrent;
using Binacle.Net.ServiceModule.Application.Subscriptions.Services;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Entities;
using FluxResults.TypedResults;
using FluxResults.Unions;

namespace Binacle.Net.ServiceModule.Infrastructure.Subscriptions.Services;

internal class InMemorySubscriptionRepository : ISubscriptionRepository
{
	private static readonly ConcurrentDictionary<Guid, Subscription> _subscriptions = new();
	
	public Task<FluxUnion<Subscription, NotFound>> GetByIdAsync(Guid id)
	{
		if (_subscriptions.TryGetValue(id, out var subscription))
		{
			return Task.FromResult<FluxUnion<Subscription, NotFound>>(subscription);
		}

		return Task.FromResult<FluxUnion<Subscription, NotFound>>(TypedResult.NotFound);
	}

	public Task<FluxUnion<Subscription, NotFound>> GetByAccountIdAsync(Guid accountId)
	{
		var subscription = _subscriptions.Values.FirstOrDefault(s => s.AccountId == accountId);
		if (subscription is not null)
		{
			return Task.FromResult<FluxUnion<Subscription, NotFound>>(subscription);
		}

		return Task.FromResult<FluxUnion<Subscription, NotFound>>(TypedResult.NotFound);
	}

	public Task<FluxUnion<Success, Conflict>> CreateAsync(Subscription subscription)
	{
		var added = _subscriptions.TryAdd(subscription.Id, subscription);
		return Task.FromResult<FluxUnion<Success, Conflict>>(
			added ? TypedResult.Success : TypedResult.Conflict
		);
	}

	public Task<FluxUnion<Success, NotFound>> UpdateAsync(Subscription subscription)
	{
		if (_subscriptions.TryGetValue(subscription.Id, out var existing))
		{
			var updated = _subscriptions.TryUpdate(subscription.Id, subscription, existing);
			if (updated)
			{
				return Task.FromResult<FluxUnion<Success, NotFound>>(TypedResult.Success);
			}
		}

		return Task.FromResult<FluxUnion<Success, NotFound>>(TypedResult.NotFound);
	}

	public Task<FluxUnion<Success, NotFound>> DeleteAsync(Subscription subscription)
	{
		var removed = _subscriptions.TryRemove(subscription.Id, out _);
		return Task.FromResult<FluxUnion<Success, NotFound>>(
			removed ? TypedResult.Success : TypedResult.NotFound
		);
	}
}
