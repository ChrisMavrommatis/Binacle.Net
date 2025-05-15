using System.Text.Json.Serialization;
using Azure;
using Azure.Data.Tables;
using Binacle.Net.ServiceModule.Domain.Common.Models;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Entities;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Models;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Services;
using Binacle.Net.ServiceModule.Infrastructure.Common.ExtensionMethods;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Conflict = FluxResults.Conflict;
using NotFound = FluxResults.NotFound;

namespace Binacle.Net.ServiceModule.Infrastructure.Subscriptions.Services;

internal class AzureTablesSubscriptionRepository : ISubscriptionRepository
{
	private readonly TableClient tableClient;
	internal const string TablePartitionKey = "Subscriptions";
	internal const string TableName = "subscriptions";

	public AzureTablesSubscriptionRepository(
		TableServiceClient tableServiceClient
	)
	{
		this.tableClient = tableServiceClient.GetTableClient(TableName);
	}

	public async Task<FluxUnion<Subscription, NotFound>> GetByIdAsync(
		Guid id,
		bool allowDeleted = false,
		CancellationToken cancellationToken = default
	)
	{
		var response = await this.tableClient.GetEntityIfExistsAsync<SubscriptionTableEntity>(
			TablePartitionKey,
			id.ToString(),
			cancellationToken: cancellationToken
		);
		if (response is null || !response.HasValue)
		{
			return TypedResult.NotFound;
		}

		if (!allowDeleted && response.Value!.IsDeleted)
		{
			return TypedResult.NotFound;
		}

		var domainEntity = response.Value!.ToDomain();
		return domainEntity;
	}

	public Task<PagedList<Subscription>> ListAsync(
		int page,
		int pageSize,
		CancellationToken cancellationToken = default
	)
	{
		throw new NotImplementedException();
	}

	public async Task<FluxUnion<Subscription, NotFound>> GetByAccountIdAsync(
		Guid accountId,
		CancellationToken cancellationToken = default
	)
	{
		var pageableResult = this.tableClient.QueryAsync<SubscriptionTableEntity>(
			x => x.AccountId == accountId && x.IsDeleted == false,
			maxPerPage: 1,
			cancellationToken: cancellationToken
		);
		var result = await pageableResult.FirstOrDefaultAsync(cancellationToken);
		if (result is not null)
		{
			return result.ToDomain();
		}

		return TypedResult.NotFound;
	}

	public async Task<FluxUnion<Success, Conflict>> CreateAsync(
		Subscription subscription,
		CancellationToken cancellationToken = default
	)
	{
		var entity = new SubscriptionTableEntity(subscription);
		try
		{
			var response = await tableClient.AddEntityAsync(entity, cancellationToken: cancellationToken);

			int[] successStatusCodes = [StatusCodes.Status201Created, StatusCodes.Status204NoContent];
			var isSuccess = successStatusCodes.Contains(response.Status) && !response.IsError;
			if (isSuccess)
				return TypedResult.Success;
			
			return TypedResult.Conflict;
		}
		catch (Azure.RequestFailedException)
		{
			return TypedResult.Conflict;
		}
	}

	public async Task<FluxUnion<Success, NotFound>> UpdateAsync(
		Subscription subscription,
		CancellationToken cancellationToken = default
	)
	{
		var entity = new SubscriptionTableEntity(subscription);
		var response = await tableClient.UpdateEntityAsync(
			entity,
			Azure.ETag.All,
			TableUpdateMode.Replace,
			cancellationToken: cancellationToken
		);

		var isSuccess = response.Status == StatusCodes.Status204NoContent && !response.IsError;
		if (isSuccess)
			return TypedResult.Success;

		return TypedResult.NotFound;
	}

	public async Task<FluxUnion<Success, NotFound>> DeleteAsync(
		Subscription subscription,
		CancellationToken cancellationToken = default
	)
	{
		var response = await this.tableClient.DeleteEntityAsync(
			TablePartitionKey,
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
		#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
		public SubscriptionTableEntity()
		{
		}

		public string PartitionKey { get; set; }
		public string RowKey { get; set; }
		public DateTimeOffset? Timestamp { get; set; }
		public ETag ETag { get; set; }

		public Guid AccountId { get; set; }

		[JsonConverter(typeof(JsonStringEnumConverter))]
		public SubscriptionType Type { get; set; }

		[JsonConverter(typeof(JsonStringEnumConverter))]
		public SubscriptionStatus Status { get; set; }

		public DateTimeOffset CreatedAtUtc { get; set; }
		public bool IsDeleted { get; set; }
		public DateTimeOffset? DeletedAtUtc { get; set; }

		public SubscriptionTableEntity(Subscription subscription)
		{
			this.RowKey = subscription.Id.ToString();
			this.PartitionKey = AzureTablesSubscriptionRepository.TablePartitionKey;
			this.AccountId = subscription.AccountId;
			this.Type = subscription.Type;
			this.Status = subscription.Status;
			this.CreatedAtUtc = subscription.CreatedAtUtc;
			this.IsDeleted = subscription.IsDeleted;
			this.DeletedAtUtc = subscription.DeletedAtUtc;
		}

		public Subscription ToDomain()
		{
			var subscription = new Subscription(
				this.AccountId,
				this.Status,
				this.Type,
				this.CreatedAtUtc,
				Guid.Parse(this.RowKey),
				this.IsDeleted,
				this.DeletedAtUtc
			);
			return subscription;
		}
	}
}
