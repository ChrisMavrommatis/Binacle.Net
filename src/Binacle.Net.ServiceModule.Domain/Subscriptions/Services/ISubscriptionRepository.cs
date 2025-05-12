using Binacle.Net.ServiceModule.Domain.Common.Models;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Entities;
using FluxResults.TypedResults;
using FluxResults.Unions;

namespace Binacle.Net.ServiceModule.Domain.Subscriptions.Services;

public interface ISubscriptionRepository
{
	Task<FluxUnion<Subscription, NotFound>> GetByIdAsync(Guid id, bool allowDeleted = false, CancellationToken cancellationToken = default);
	Task<PagedList<Subscription>> ListAsync(int page, int pageSize, CancellationToken cancellationToken = default);
	Task<FluxUnion<Subscription, NotFound>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
	Task<FluxUnion<Success, Conflict>> CreateAsync(Subscription subscription, CancellationToken cancellationToken = default);
	Task<FluxUnion<Success, NotFound>> UpdateAsync(Subscription subscription, CancellationToken cancellationToken = default);
	Task<FluxUnion<Success, NotFound>> DeleteAsync(Subscription subscription, CancellationToken cancellationToken = default);
}

