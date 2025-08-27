using System.Data;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Entities;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Models;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Services;
using Dapper;

namespace Binacle.Net.ServiceModule.Infrastructure.Subscriptions.Services;

internal class SqliteSubscriptionRepository : ISubscriptionRepository
{
	private readonly IDbConnection connection;

	public SqliteSubscriptionRepository(IDbConnection connection)
	{
		this.connection = connection;
	}

	public Task<FluxUnion<Subscription, NotFound>> GetByIdAsync(Guid id, bool allowDeleted = false, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public Task<FluxUnion<Subscription, NotFound>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public Task<FluxUnion<Success, Conflict>> CreateAsync(Subscription subscription, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public Task<FluxUnion<Success, NotFound>> UpdateAsync(Subscription subscription, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public Task<FluxUnion<Success, NotFound>> DeleteAsync(Subscription subscription, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	private class SubscriptionDto
	{
		#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
		public string Id { get; set; }
		public string AccountId { get; set; }
		public string Type { get; set; }
		public string Status { get; set; }
		public string CreatedAtUtc { get; set; }
		public int IsDeleted { get; set; }
		public string? DeletedAtUtc { get; set; }
		
		#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
		
		public Subscription ToDomain()
		{
			return new Subscription(
				accountId: Guid.Parse(this.AccountId),
				status: Enum.Parse<SubscriptionStatus>(this.Status),
				type: Enum.Parse<SubscriptionType>(this.Type),
				creationDate: DateTimeOffset.Parse(this.CreatedAtUtc),
				id: Guid.Parse(this.Id),
				isDeleted: this.IsDeleted == 1,
				deletionDate: this.DeletedAtUtc != null ? DateTimeOffset.Parse(this.DeletedAtUtc) : null
			);
		}

		public static SubscriptionDto FromDomain(Subscription entity)
		{
			return new SubscriptionDto
			{
				Id = entity.Id.ToString(),
				AccountId = entity.AccountId.ToString(),
				Status = entity.Status.ToString(),
				Type = entity.Type.ToString(),
				CreatedAtUtc = entity.CreatedAtUtc.ToString(),
				IsDeleted = entity.IsDeleted ? 1 : 0,
				DeletedAtUtc = entity.DeletedAtUtc?.ToString()
			};
		}
		
	}


	internal static async Task EnsureTableExistsAsync(IDbConnection connection)
	{
		const string sql = @"
			CREATE TABLE IF NOT EXISTS Subscriptions (
				Id TEXT PRIMARY KEY,
				AccountId TEXT NOT NULL,
				Status TEXT NOT NULL,
				Type TEXT NOT NULL,
				CreatedAtUtc TEXT NOT NULL,
				IsDeleted INTEGER NOT NULL DEFAULT 0,
				DeletedAtUtc TEXT
			);
		";
		int rowsAffected = await connection.ExecuteAsync(sql);
	}
}
