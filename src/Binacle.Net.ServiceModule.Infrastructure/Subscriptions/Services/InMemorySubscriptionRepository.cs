using System.Collections.Concurrent;
using Binacle.Net.ServiceModule.Domain.Common.Models;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Entities;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Services;
using Binacle.Net.ServiceModule.Infrastructure.Common.Models;
using FluxResults.TypedResults;
using FluxResults.Unions;

namespace Binacle.Net.ServiceModule.Infrastructure.Subscriptions.Services;

internal class InMemorySubscriptionRepository : ISubscriptionRepository
{
	private static readonly ConcurrentSortedDictionary<Guid, Subscription> _subscriptions = new();
	
	public Task<FluxUnion<Subscription, NotFound>> GetByIdAsync(Guid id)
	{
		if (_subscriptions.TryGetValue(id, out var subscription) && !subscription.IsDeleted)
		{
			return Task.FromResult<FluxUnion<Subscription, NotFound>>(subscription);
		}

		return Task.FromResult<FluxUnion<Subscription, NotFound>>(TypedResult.NotFound);
	}
	
	public Task<FluxUnion<PagedList<Subscription>, NotFound>> ListAsync(int page, int pageSize)
	{
		var subscriptions = _subscriptions.Values
			.Where(x => !x.IsDeleted)
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.ToList();

		if (subscriptions.Count == 0)
		{
			return Task.FromResult<FluxUnion<PagedList<Subscription>, NotFound>>(TypedResult.NotFound);
		}

		var pagedSubscriptions = new PagedList<Subscription>(
			subscriptions,
			subscriptions.Count,
			pageSize,
			page
		);

		return Task.FromResult<FluxUnion<PagedList<Subscription>, NotFound>>(pagedSubscriptions);
	}


	public Task<FluxUnion<Subscription, NotFound>> GetByAccountIdAsync(Guid accountId)
	{
		var subscription = _subscriptions.Values.FirstOrDefault(s => s.AccountId == accountId);
		if (subscription is not null && !subscription.IsDeleted)
		{
			return Task.FromResult<FluxUnion<Subscription, NotFound>>(subscription);
		}

		return Task.FromResult<FluxUnion<Subscription, NotFound>>(TypedResult.NotFound);
	}

	public Task<FluxUnion<Success, Conflict>> CreateAsync(Subscription subscription)
	{
		if(_subscriptions.ContainsKey(subscription.Id))
		{
			return Task.FromResult<FluxUnion<Success, Conflict>>(TypedResult.Conflict);
		}
		_subscriptions.Add(subscription.Id, subscription);
		return Task.FromResult<FluxUnion<Success, Conflict>>(TypedResult.Success);
	}

	public Task<FluxUnion<Success, NotFound>> UpdateAsync(Subscription subscription)
	{
		if (_subscriptions.TryGetValue(subscription.Id, out var existing) && !existing.IsDeleted)
		{
			_subscriptions[subscription.Id] = subscription;
			return Task.FromResult<FluxUnion<Success, NotFound>>(TypedResult.Success);
		}

		return Task.FromResult<FluxUnion<Success, NotFound>>(TypedResult.NotFound);
	}
	
	public Task<FluxUnion<Success, NotFound>> ForceUpdateAsync(Subscription subscription)
	{
		if (_subscriptions.TryGetValue(subscription.Id, out var existing))
		{
			_subscriptions[subscription.Id] = subscription;
			return Task.FromResult<FluxUnion<Success, NotFound>>(TypedResult.Success);
		}

		return Task.FromResult<FluxUnion<Success, NotFound>>(TypedResult.NotFound);
	}

	public Task<FluxUnion<Success, NotFound>> DeleteAsync(Subscription subscription)
	{
		var removed = _subscriptions.Remove(subscription.Id);
		return Task.FromResult<FluxUnion<Success, NotFound>>(
			removed ? TypedResult.Success : TypedResult.NotFound
		);
	}
}
