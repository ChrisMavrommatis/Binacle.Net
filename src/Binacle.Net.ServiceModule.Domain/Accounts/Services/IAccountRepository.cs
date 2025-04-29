using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using Binacle.Net.ServiceModule.Domain.Common.Models;
using FluxResults.TypedResults;
using FluxResults.Unions;

namespace Binacle.Net.ServiceModule.Domain.Accounts.Services;

public interface IAccountRepository
{
	Task<FluxUnion<Account, NotFound>> GetByIdAsync(Guid id);
	Task<FluxUnion<PagedList<Account>, NotFound>> ListAsync(int page, int pageSize);
	Task<FluxUnion<Account, NotFound>> GetByUsernameAsync(string username);
	Task<FluxUnion<Success, Conflict>> CreateAsync(Account account);
	Task<FluxUnion<Success, NotFound>> UpdateAsync(Account account);
	Task<FluxUnion<Success, NotFound>> ForceUpdateAsync(Account account);
	Task<FluxUnion<Success, NotFound>> DeleteAsync(Account account);
}

