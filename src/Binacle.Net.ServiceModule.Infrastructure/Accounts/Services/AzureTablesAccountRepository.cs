using Azure.Data.Tables;
using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using Binacle.Net.ServiceModule.Domain.Accounts.Services;
using Binacle.Net.ServiceModule.Domain.Common.Models;
using FluxResults.TypedResults;
using FluxResults.Unions;

namespace Binacle.Net.ServiceModule.Infrastructure.Accounts.Services;

internal class AzureTablesAccountRepository : IAccountRepository
{
	private readonly TableClient tableClient;
	internal const string PartitionKey = "Accounts";
	internal const string TableName = "accounts";

	public AzureTablesAccountRepository(
		TableServiceClient tableServiceClient
	)
	{
		this.tableClient = tableServiceClient.GetTableClient(TableName);
	}
	public Task<FluxUnion<Account, NotFound>> GetByIdAsync(Guid id, bool allowDeleted = false, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public Task<PagedList<Account>> ListAsync(int page, int pageSize, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public Task<FluxUnion<Account, NotFound>> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public Task<FluxUnion<Success, Conflict>> CreateAsync(Account account, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public Task<FluxUnion<Success, NotFound>> UpdateAsync(Account account, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public Task<FluxUnion<Success, NotFound>> DeleteAsync(Account account, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}
