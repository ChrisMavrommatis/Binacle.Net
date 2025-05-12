using System.Runtime.InteropServices.JavaScript;
using Azure;
using Azure.Data.Tables;
using Binacle.Net.ServiceModule.Domain.Common.Models;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Entities;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Services;
using FluxResults.TypedResults;
using FluxResults.Unions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Conflict = FluxResults.TypedResults.Conflict;
using NotFound = FluxResults.TypedResults.NotFound;

namespace Binacle.Net.ServiceModule.Infrastructure.Subscriptions.Services;

internal class AzureTablesSubscriptionRepository : ISubscriptionRepository
{
	private readonly TableClient tableClient;
	internal const string PartitionKey = "Subscriptions";
	internal const string TableName = "subscriptions";

	public AzureTablesSubscriptionRepository(
		TableServiceClient tableServiceClient
	)
	{
		this.tableClient = tableServiceClient.GetTableClient(TableName);
	}

	public async Task<FluxUnion<Subscription, NotFound>> GetByIdAsync(Guid id, bool allowDeleted = false,
		CancellationToken cancellationToken = default)
	{
		var response = await this.tableClient.GetEntityIfExistsAsync<SubscriptionTableEntity>(
			PartitionKey,
			id.ToString(),
			cancellationToken: cancellationToken
		);
		if (response is not null && response.HasValue)
		{
			var domainEntity = response.Value!.ToDomain();
			return domainEntity;
		}

		return TypedResult.NotFound;
	}

	public async Task<PagedList<Subscription>> ListAsync(int page, int pageSize,
		CancellationToken cancellationToken = default)
	{
		var pageableResult = this.tableClient.QueryAsync<SubscriptionTableEntity>(
			filter: string.Empty,
			maxPerPage: pageSize,
			cancellationToken: cancellationToken
		);
		throw new NotImplementedException();
	}

	public async Task<FluxUnion<Subscription, NotFound>> GetByAccountIdAsync(Guid accountId,
		CancellationToken cancellationToken = default)
	{
	
		throw new NotImplementedException();

	}

	public async Task<FluxUnion<Success, Conflict>> CreateAsync(Subscription subscription,
		CancellationToken cancellationToken = default)
	{
		
		var entity = new SubscriptionTableEntity(subscription);
		var response = await tableClient.AddEntityAsync(entity, cancellationToken: cancellationToken);

		int[] successStatusCodes = [StatusCodes.Status201Created, StatusCodes.Status204NoContent];
		var isSuccess = successStatusCodes.Contains(response.Status) && !response.IsError;
		if (isSuccess)
			return TypedResult.Success;

		return TypedResult.Conflict;
	}

	public async Task<FluxUnion<Success, NotFound>> UpdateAsync(Subscription subscription,
		CancellationToken cancellationToken = default)
	{
		var entity = new SubscriptionTableEntity(subscription);
		var response = await tableClient.UpdateEntityAsync(
			entity,
			Azure.ETag.All,
			cancellationToken: cancellationToken
		);

		var isSuccess = response.Status == StatusCodes.Status204NoContent && !response.IsError;
		if (isSuccess)
			return TypedResult.Success;

		return TypedResult.NotFound;
	}

	public async Task<FluxUnion<Success, NotFound>> DeleteAsync(Subscription subscription,
		CancellationToken cancellationToken = default)
	{
		var response = await this.tableClient.DeleteEntityAsync(
			PartitionKey, 
			subscription.Id.ToString(),
			cancellationToken: cancellationToken
		);

		var isSuccess = response.Status == StatusCodes.Status204NoContent && !response.IsError;
		if (isSuccess)
			return TypedResult.Success;

		return TypedResult.NotFound;
	}


	private class SubscriptionTableEntity : ITableEntity
	{
		public SubscriptionTableEntity()
		{
		}

		public string PartitionKey { get; set; }
		public string RowKey { get; set; }
		public DateTimeOffset? Timestamp { get; set; }
		public ETag ETag { get; set; }

		public SubscriptionTableEntity(Subscription subscription)
		{
			this.RowKey = subscription.Id.ToString();
			this.PartitionKey = AzureTablesSubscriptionRepository.PartitionKey;
		}

		public Subscription ToDomain()
		{
			throw new NotImplementedException();
		}
	}
}
