using Azure.Data.Tables;
using Binacle.Net.Api.ServiceModule.Domain.Users.Data;
using Binacle.Net.Api.ServiceModule.Domain.Users.Entities;
using Binacle.Net.Api.ServiceModule.Infrastructure.AzureTables.Users.Entities;
using ChrisMavrommatis.Results.Typed;
using ChrisMavrommatis.Results.Unions;
using Microsoft.AspNetCore.Http;

namespace Binacle.Net.Api.ServiceModule.Infrastructure.AzureTables.Users.Data;

public class AzureTablesUserRepository : IUserRepository
{
	private readonly TableServiceClient tableServiceClient;
	private const string _partitionKey = UsersTablePartitionKeys.Users;
	private const string _tableName = TableNames.Users;

	public AzureTablesUserRepository(
		TableServiceClient tableServiceClient
		)
	{
		this.tableServiceClient = tableServiceClient;
	}

	public async Task<OneOf<Ok, Error>> CreateAsync(User user, CancellationToken cancellationToken = default)
	{
		await this.tableServiceClient.CreateTableIfNotExistsAsync(_tableName);
		var tableClient = this.tableServiceClient.GetTableClient(_tableName);

		var tableEntity = user.ToTableEntityUser(_partitionKey);

		var response = await tableClient.AddEntityAsync(tableEntity, cancellationToken: cancellationToken);

		var isSuccess = (new[] { StatusCodes.Status201Created, StatusCodes.Status204NoContent }).Contains(response.Status) && !response.IsError;
		if (isSuccess)
			return new Ok();

		return new Error(response.ReasonPhrase);
	}

	public async Task<OneOf<User, Error>> GetAsync(string email, CancellationToken cancellationToken = default)
	{
		var tableClient = this.tableServiceClient.GetTableClient(_tableName);

		var response = await tableClient.GetEntityIfExistsAsync<UserTableEntity>(_partitionKey, email.ToLowerInvariant(), cancellationToken: cancellationToken);

		if (response is not null && response.HasValue)
		{
			var domainEntity = response.Value!.ToDomainUser();
			return domainEntity;
		}

		return new Error("User not found");
	}

	public async Task<OneOf<Ok, Error>> DeleteAsync(string email, CancellationToken cancellationToken = default)
	{
		var tableClient = this.tableServiceClient.GetTableClient(_tableName);

		var response = await tableClient.DeleteEntityAsync(_partitionKey, email.ToLowerInvariant(), cancellationToken: cancellationToken);

		var isSuccess = response.Status == StatusCodes.Status204NoContent && !response.IsError;
		if (isSuccess)
			return new Ok();

		return new Error(response.ReasonPhrase);
	}

	public async Task<OneOf<Ok, Error>> UpdateAsync(User user, CancellationToken cancellationToken = default)
	{
		var tableClient = this.tableServiceClient.GetTableClient(_tableName);

		var tableEntity = user.ToTableEntityUser(_partitionKey);

		var response = await tableClient.UpdateEntityAsync(tableEntity, Azure.ETag.All, cancellationToken: cancellationToken);

		var isSuccess = response.Status == StatusCodes.Status204NoContent && !response.IsError;
		if (isSuccess)
			return new Ok();

		return new Error(response.ReasonPhrase);
	}
}
