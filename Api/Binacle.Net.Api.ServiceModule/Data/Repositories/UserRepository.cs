using Azure.Data.Tables;
using Binacle.Net.Api.ServiceModule.Data.Entities;
using Binacle.Net.Api.ServiceModule.Data.Schemas;
using Microsoft.AspNetCore.Http;

namespace Binacle.Net.Api.ServiceModule.Data.Repositories;

internal interface IUserRepository
{
	Task<UserEntity?> GetAsync(string email, CancellationToken cancellationToken = default);
	Task<bool> CreateAsync(UserEntity user, CancellationToken cancellationToken = default);
}

internal class UserRepository : IUserRepository
{
	private readonly TableServiceClient tableServiceClient;

	public UserRepository(
		TableServiceClient tableServiceClient
		)
	{
		this.tableServiceClient = tableServiceClient;
	}

	public async Task<bool> CreateAsync(UserEntity user, CancellationToken cancellationToken = default)
	{
		await this.tableServiceClient.CreateTableIfNotExistsAsync(TableNames.Users);
		var tableClient = this.tableServiceClient.GetTableClient(TableNames.Users);
		var result = await tableClient.AddEntityAsync(user, cancellationToken: cancellationToken);
		return result.Status == StatusCodes.Status201Created && !result.IsError;
	}

	public async Task<UserEntity?> GetAsync(string email, CancellationToken cancellationToken = default)
	{
		var tableClient = this.tableServiceClient.GetTableClient(TableNames.Users);

		var response = await tableClient.GetEntityIfExistsAsync<UserEntity>(UserGroups.Users, email, cancellationToken: cancellationToken);
		if (!response.HasValue)
			return null;

		return response.Value;
	}
}
