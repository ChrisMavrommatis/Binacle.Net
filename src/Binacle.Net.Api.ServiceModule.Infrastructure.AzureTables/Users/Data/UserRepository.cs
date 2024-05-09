﻿using Azure.Data.Tables;
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

	public AzureTablesUserRepository(
		TableServiceClient tableServiceClient
		)
	{
		this.tableServiceClient = tableServiceClient;
	}

	public async Task<OneOf<Ok, Error>> CreateAsync(User user, CancellationToken cancellationToken = default)
	{
		await this.tableServiceClient.CreateTableIfNotExistsAsync(TableNames.Users);
		var tableClient = this.tableServiceClient.GetTableClient(TableNames.Users);

		var infrastructureEntity = this.MapToInfrastructureModel(user);

		var response = await tableClient.AddEntityAsync(infrastructureEntity, cancellationToken: cancellationToken);

		var isSuccess = (new[] { StatusCodes.Status201Created, StatusCodes.Status204NoContent }).Contains(response.Status) && !response.IsError;
		if (isSuccess)
			return new Ok();

		return new Error(response.ReasonPhrase);
	}

	public async Task<OneOf<User, Error>> GetAsync(string email, CancellationToken cancellationToken = default)
	{
		var tableClient = this.tableServiceClient.GetTableClient(TableNames.Users);

		var response = await tableClient.GetEntityIfExistsAsync<UserTableEntity>(UserGroups.Users, email, cancellationToken: cancellationToken);

		if (response is not null && response.HasValue)
		{
			var domainEntity = this.MapToDomainModel(response.Value!);
			return domainEntity;
		}

		return new Error("User not found");
	}

	public async Task<OneOf<Ok, Error>> DeleteAsync(string email, CancellationToken cancellationToken = default)
	{
		var tableClient = this.tableServiceClient.GetTableClient(TableNames.Users);

		var response = await tableClient.DeleteEntityAsync(UserGroups.Users, email, cancellationToken: cancellationToken);

		var isSuccess = response.Status == StatusCodes.Status204NoContent && !response.IsError;
		if (isSuccess)
			return new Ok();

		return new Error(response.ReasonPhrase);
	}

	public async Task<OneOf<Ok, Error>> UpdateAsync(User user, CancellationToken cancellationToken = default)
	{
		var tableClient = this.tableServiceClient.GetTableClient(TableNames.Users);


		var infrastructureEntity = this.MapToInfrastructureModel(user);

		var response = await tableClient.UpdateEntityAsync(infrastructureEntity, Azure.ETag.All, cancellationToken: cancellationToken);

		var isSuccess = response.Status == StatusCodes.Status204NoContent && !response.IsError;
		if (isSuccess)
			return new Ok();

		return new Error(response.ReasonPhrase);
	}

	private User MapToDomainModel(UserTableEntity infrastructureEntity)
	{
		return new User
		{
			Email = infrastructureEntity.Email,
			Group = infrastructureEntity.Group,
			Salt = infrastructureEntity.Salt,
			HashedPassword = infrastructureEntity.HashedPassword
		};
	}

	private UserTableEntity MapToInfrastructureModel(User domainEntity)
	{
		return new UserTableEntity(domainEntity.Email, domainEntity.Group)
		{
			Salt = domainEntity.Salt,
			HashedPassword = domainEntity.HashedPassword
		};
	}
}
