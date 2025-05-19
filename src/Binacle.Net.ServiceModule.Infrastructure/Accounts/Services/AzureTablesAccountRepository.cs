using System.Text.Json.Serialization;
using Azure;
using Azure.Data.Tables;
using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using Binacle.Net.ServiceModule.Domain.Accounts.Models;
using Binacle.Net.ServiceModule.Domain.Accounts.Services;
using Binacle.Net.ServiceModule.Domain.Common.Models;
using Binacle.Net.ServiceModule.Infrastructure.Common.ExtensionMethods;
using Microsoft.AspNetCore.Http;

namespace Binacle.Net.ServiceModule.Infrastructure.Accounts.Services;

internal class AzureTablesAccountRepository : IAccountRepository
{
	private readonly TableClient tableClient;
	internal const string TablePartitionKey = "Accounts";
	internal const string TableName = "accounts";

	public AzureTablesAccountRepository(
		TableServiceClient tableServiceClient
	)
	{
		this.tableClient = tableServiceClient.GetTableClient(TableName);
	}
	public async Task<FluxUnion<Account, NotFound>> GetByIdAsync(
		Guid id,
		bool allowDeleted = false,
		CancellationToken cancellationToken = default
	)
	{
		var response = await this.tableClient.GetEntityIfExistsAsync<AccountTableEntity>(
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

	public async Task<FluxUnion<Account, NotFound>> GetByUsernameAsync(
		string username, 
		CancellationToken cancellationToken = default
	)
	{
		var pageableResult = this.tableClient.QueryAsync<AccountTableEntity>(
			x => x.Username == username && x.IsDeleted == false,
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
		Account account, 
		CancellationToken cancellationToken = default
	)
	{
		var entity = new AccountTableEntity(account);
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
		Account account, 
		CancellationToken cancellationToken = default
	)
	{
		var entity = new AccountTableEntity(account);
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

	public async Task<FluxUnion<Success, NotFound>> DeleteAsync(
		Account account, 
		CancellationToken cancellationToken = default
	)
	{
		var response = await this.tableClient.DeleteEntityAsync(
			TablePartitionKey,
			account.Id.ToString(),
			cancellationToken: cancellationToken
		);

		var isSuccess = response.Status == StatusCodes.Status204NoContent && !response.IsError;
		if (isSuccess)
			return TypedResult.Success;

		return TypedResult.NotFound;
	}
	
	private class AccountTableEntity : ITableEntity
	{
		#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
		public AccountTableEntity()
		{
		}

		public string PartitionKey { get; set; }
		public string RowKey { get; set; }
		public DateTimeOffset? Timestamp { get; set; }
		public ETag ETag { get; set; }

		public string Username { get;  set; }
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public AccountRole Role { get; set; }
		public string Email { get; set; }
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public AccountStatus Status { get; set; }
		public string? Password { get; set; }
		public Guid SecurityStamp { get; set; }
		public Guid? SubscriptionId { get; set; }
		public DateTimeOffset CreatedAtUtc { get; set; }
		public bool IsDeleted { get; set; }
		public DateTimeOffset? DeletedAtUtc { get; set; }
		
		public AccountTableEntity(Account account)
		{
			this.RowKey = account.Id.ToString();
			this.PartitionKey = AzureTablesAccountRepository.TablePartitionKey;
			this.Username = account.Username;
			this.Role = account.Role;
			this.Email = account.Email;
			this.Status = account.Status;
			this.Password = account.Password?.ToString();
			this.SecurityStamp = account.SecurityStamp;
			this.SubscriptionId = account.SubscriptionId;
			this.CreatedAtUtc = account.CreatedAtUtc;
			this.IsDeleted = account.IsDeleted;
			this.DeletedAtUtc = account.DeletedAtUtc;
		}

		public Account ToDomain()
		{
			var password = Domain.Common.Models.Password.TryParse(this.Password);
			
			var account = new Account(
				this.Username,
				this.Role,
				this.Email,
				this.Status,
				this.CreatedAtUtc,
				Guid.Parse(this.RowKey),
				this.IsDeleted,
				this.SecurityStamp,
				password,
				this.DeletedAtUtc,
				this.SubscriptionId
			);

			return account;
		}
	}
}
