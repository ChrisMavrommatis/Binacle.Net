using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using Binacle.Net.ServiceModule.Domain.Common.Models;

namespace Binacle.Net.ServiceModule.Domain.Accounts.Services;

public interface IAccountRepository
{
	Task<FluxUnion<Account, NotFound>> GetByIdAsync(Guid id, bool allowDeleted = false, CancellationToken cancellationToken = default);
	Task<FluxUnion<Account, NotFound>> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
	Task<FluxUnion<Success, Conflict>> CreateAsync(Account account, CancellationToken cancellationToken = default);
	Task<FluxUnion<Success, NotFound>> UpdateAsync(Account account, CancellationToken cancellationToken = default);
	Task<FluxUnion<Success, NotFound>> DeleteAsync(Account account, CancellationToken cancellationToken = default);
}

