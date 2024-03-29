using Azure.Data.Tables;
using Binacle.Net.Api.ServiceModule.Data.Entities;
using Binacle.Net.Api.ServiceModule.Data.Models;
using Binacle.Net.Api.ServiceModule.Data.Schemas;
using Microsoft.AspNetCore.Http;

namespace Binacle.Net.Api.ServiceModule.Data.Repositories;

internal interface IUserRepository
{
	Task<Result<UserEntity>> GetAsync(string email, CancellationToken cancellationToken = default);
	Task<Result> CreateAsync(UserEntity user, CancellationToken cancellationToken = default);
	Task<Result> DeleteAsync(string email, CancellationToken cancellationToken = default);
	Task<Result> UpdateAsync(UserEntity user, CancellationToken cancellationToken = default);
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

	public async Task<Result> CreateAsync(UserEntity user, CancellationToken cancellationToken = default)
	{
		await this.tableServiceClient.CreateTableIfNotExistsAsync(TableNames.Users);
		var tableClient = this.tableServiceClient.GetTableClient(TableNames.Users);

		var response = await tableClient.AddEntityAsync(user, cancellationToken: cancellationToken);

		var isSuccess = (new[] { StatusCodes.Status201Created, StatusCodes.Status204NoContent }).Contains(response.Status) && !response.IsError;
		if (isSuccess)
			return Result.Successful();

		return Result.Failed(response.ReasonPhrase);
	}

	public async Task<Result<UserEntity>> GetAsync(string email, CancellationToken cancellationToken = default)
	{
		var tableClient = this.tableServiceClient.GetTableClient(TableNames.Users);

		var response = await tableClient.GetEntityIfExistsAsync<UserEntity>(UserGroups.Users, email, cancellationToken: cancellationToken);

		if (response is not null && response.HasValue)
			return Result<UserEntity>.Successful(response.Value!);

		return Result<UserEntity>.Failed("User not found");
	}

	public async Task<Result> DeleteAsync(string email, CancellationToken cancellationToken = default)
	{
		var tableClient = this.tableServiceClient.GetTableClient(TableNames.Users);

		var response = await tableClient.DeleteEntityAsync(UserGroups.Users, email, cancellationToken: cancellationToken);

		var isSuccess = response.Status == StatusCodes.Status204NoContent && !response.IsError;
		if (isSuccess)
			return Result.Successful();

		return Result.Failed(response.ReasonPhrase);
	}

	public async Task<Result> UpdateAsync(UserEntity user, CancellationToken cancellationToken = default)
	{
		var tableClient = this.tableServiceClient.GetTableClient(TableNames.Users);

		var response = await tableClient.UpdateEntityAsync(user, Azure.ETag.All, cancellationToken: cancellationToken);

		var isSuccess = response.Status == StatusCodes.Status204NoContent && !response.IsError;
		if (isSuccess)
			return Result.Successful();

		return Result.Failed(response.ReasonPhrase);
	}
}
