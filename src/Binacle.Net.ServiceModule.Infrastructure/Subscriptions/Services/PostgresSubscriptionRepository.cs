using System.Data;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Entities;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Models;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Services;
using Dapper;

namespace Binacle.Net.ServiceModule.Infrastructure.Subscriptions.Services;

internal class PostgresSubscriptionRepository : ISubscriptionRepository
{
	private readonly IDbConnection connection;

	public PostgresSubscriptionRepository(IDbConnection connection)
	{
		this.connection = connection;
	}

	public async Task<FluxUnion<Subscription, NotFound>> GetByIdAsync(
		Guid id,
		bool allowDeleted = false,
		CancellationToken cancellationToken = default
		)
	{
		const string sql = "SELECT * FROM Subscriptions WHERE Id = @Id AND IsDeleted = FALSE LIMIT 1";
		const string allowDeletedSql = "SELECT * FROM Subscriptions WHERE Id = @Id LIMIT 1";
		var dto = await this.connection.QueryFirstOrDefaultAsync<SubscriptionDto>(
			allowDeleted ? allowDeletedSql : sql,
			new { Id = id }
		);

		if (dto is null)
		{
			return TypedResult.NotFound;
		}

		return dto.ToDomain();
	}

	public async Task<FluxUnion<Subscription, NotFound>> GetByAccountIdAsync(
		Guid accountId, 
		CancellationToken cancellationToken = default
		)
	{
		const string sql = "SELECT * FROM Subscriptions WHERE AccountId = @AccountId AND IsDeleted = FALSE LIMIT 1";

		var dto = await this.connection.QueryFirstOrDefaultAsync<SubscriptionDto>(sql, new { AccountId = accountId });

		if (dto is null)
		{
			return TypedResult.NotFound;
		}

		return dto.ToDomain();
	}

	public async Task<FluxUnion<Success, Conflict>> CreateAsync(
		Subscription subscription, 
		CancellationToken cancellationToken = default
		)
	{
		const string sql = @"
			INSERT INTO Subscriptions
			(Id, AccountId, Type, Status, CreatedAtUtc, IsDeleted, DeletedAtUtc)
			VALUES 
			(@Id, @AccountId, @Type, @Status, @CreatedAtUtc, @IsDeleted, @DeletedAtUtc);
		";
		try
		{
			var dto = SubscriptionDto.FromDomain(subscription);
			int affectedRows = await connection.ExecuteAsync(sql, dto);
			if (affectedRows != 1)
			{
				return TypedResult.Conflict;
			}

			return TypedResult.Success;
		}
		catch (Exception)
		{
			return TypedResult.Conflict;
		}
	}

	public async Task<FluxUnion<Success, NotFound>> UpdateAsync(
		Subscription subscription, 
		CancellationToken cancellationToken = default
		)
	{
		const string sql = @"
			UPDATE Subscriptions
			SET
				AccountId = @AccountId,
				Type = @Type,
				Status = @Status,
				CreatedAtUtc = @CreatedAtUtc,
				IsDeleted = @IsDeleted,
				DeletedAtUtc = @DeletedAtUtc
			WHERE Id = @Id
		";

		var dto = SubscriptionDto.FromDomain(subscription);
		var affectedRows = await connection.ExecuteAsync(sql, dto);

		if (affectedRows <= 0)
		{
			return TypedResult.NotFound;
		}

		return TypedResult.Success;
	}

	public async Task<FluxUnion<Success, NotFound>> DeleteAsync(
		Subscription subscription, 
		CancellationToken cancellationToken = default
		)
	{
		const string sql = @"DELETE FROM Subscriptions WHERE Id = @Id";
		var affectedRows = await connection.ExecuteAsync(sql, new { Id = subscription.Id });

		if (affectedRows <= 0)
		{
			return TypedResult.NotFound;
		}

		return TypedResult.Success;
	}

	private class SubscriptionDto
	{
		#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
		public Guid Id { get; set; }
		public Guid AccountId { get; set; }
		public string Type { get; set; }
		public string Status { get; set; }
		public DateTimeOffset CreatedAtUtc { get; set; }
		public bool IsDeleted { get; set; }
		public DateTimeOffset? DeletedAtUtc { get; set; }
		
		#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
		
		public Subscription ToDomain()
		{
			return new Subscription(
				accountId: this.AccountId,
				status: Enum.Parse<SubscriptionStatus>(this.Status),
				type: Enum.Parse<SubscriptionType>(this.Type),
				creationDate: this.CreatedAtUtc,
				id: this.Id,
				isDeleted: this.IsDeleted,
				deletionDate: this.DeletedAtUtc
			);
		}

		public static SubscriptionDto FromDomain(Subscription entity)
		{
			return new SubscriptionDto
			{
				Id = entity.Id,
				AccountId = entity.AccountId,
				Status = entity.Status.ToString(),
				Type = entity.Type.ToString(),
				CreatedAtUtc = entity.CreatedAtUtc,
				IsDeleted = entity.IsDeleted,
				DeletedAtUtc = entity.DeletedAtUtc
			};
		}
		
	}

	internal static async Task EnsureTableExistsAsync(IDbConnection connection)
	{
		const string sql = @"
			CREATE TABLE IF NOT EXISTS Subscriptions (
				Id UUID PRIMARY KEY,
				AccountId UUID NOT NULL,
				Status TEXT NOT NULL,
				Type TEXT NOT NULL,
				CreatedAtUtc TIMESTAMPTZ NOT NULL,
				IsDeleted BOOLEAN NOT NULL DEFAULT FALSE,
				DeletedAtUtc TIMESTAMPTZ
			);
		";
		int rowsAffected = await connection.ExecuteAsync(sql);
	}
}
