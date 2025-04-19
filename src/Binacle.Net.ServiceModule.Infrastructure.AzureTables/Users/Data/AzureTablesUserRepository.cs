using System.Runtime.InteropServices.JavaScript;
using Azure.Data.Tables;
using Binacle.Net.ServiceModule.Domain.Users.Data;
using Binacle.Net.ServiceModule.Domain.Users.Entities;
using Binacle.Net.ServiceModule.Infrastructure.AzureTables.Users.Entities;
using FluxResults;
using Microsoft.AspNetCore.Http;

namespace Binacle.Net.ServiceModule.Infrastructure.AzureTables.Users.Data;

public class AzureTablesUserRepository : IUserRepository
{
	private readonly TableClient client;
	// private readonly TableServiceClient tableServiceClient;
	private const string _partitionKey = UsersTablePartitionKeys.Users;
	private const string _tableName = TableNames.Users;

	public AzureTablesUserRepository(
		TableServiceClient tableServiceClient
		)
	{
		
		this.client = tableServiceClient.GetTableClient(_tableName);
	}

	public async Task<FluxResult<Success>> CreateAsync(User user, CancellationToken cancellationToken = default)
	{
		// await this.tableServiceClient.CreateTableIfNotExistsAsync(_tableName);

		var tableEntity = user.ToTableEntityUser(_partitionKey);

		var response = await this.client.AddEntityAsync(tableEntity, cancellationToken: cancellationToken);

		var isSuccess = (new[] { StatusCodes.Status201Created, StatusCodes.Status204NoContent }).Contains(response.Status) && !response.IsError;
		if (isSuccess)
			return Result.Success;
		
		return Result.Conflict;
	}

	public async Task<FluxResult<User>> GetAsync(string email, CancellationToken cancellationToken = default)
	{
		var response = await this.client.GetEntityIfExistsAsync<UserTableEntity>(_partitionKey, email.ToLowerInvariant(), cancellationToken: cancellationToken);

		if (response is not null && response.HasValue)
		{
			var domainEntity = response.Value!.ToDomainUser();
			return domainEntity;
		}

		return Result.NotFound;
	}

	public async Task<FluxResult<Success>> DeleteAsync(string email, CancellationToken cancellationToken = default)
	{
		var response = await this.client.DeleteEntityAsync(_partitionKey, email.ToLowerInvariant(), cancellationToken: cancellationToken);

		var isSuccess = response.Status == StatusCodes.Status204NoContent && !response.IsError;
		if (isSuccess)
			return Result.Success;

		return Result.NotFound;
	}

	public async Task<FluxResult<Success>> UpdateAsync(User user, CancellationToken cancellationToken = default)
	{
		var tableEntity = user.ToTableEntityUser(_partitionKey);

		var response = await this.client.UpdateEntityAsync(tableEntity, Azure.ETag.All, cancellationToken: cancellationToken);

		var isSuccess = response.Status == StatusCodes.Status204NoContent && !response.IsError;
		if (isSuccess)
			return Result.Success;

		return Result.NotFound;
	}
}
