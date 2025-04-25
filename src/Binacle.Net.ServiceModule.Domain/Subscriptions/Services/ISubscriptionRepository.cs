using Binacle.Net.ServiceModule.Domain.Subscriptions.Entities;
using FluxResults.TypedResults;
using FluxResults.Unions;

namespace Binacle.Net.ServiceModule.Domain.Subscriptions.Services;

public interface ISubscriptionRepository
{
	Task<FluxUnion<Subscription, NotFound>> GetByIdAsync(Guid id);
	Task<FluxUnion<Subscription, NotFound>> GetByAccountIdAsync(Guid accountId);
	Task<FluxUnion<Success, Conflict>> CreateAsync(Subscription subscription);
	Task<FluxUnion<Success, NotFound>> UpdateAsync(Subscription subscription);
	Task<FluxUnion<Success, NotFound>> DeleteAsync(Subscription subscription);
}

