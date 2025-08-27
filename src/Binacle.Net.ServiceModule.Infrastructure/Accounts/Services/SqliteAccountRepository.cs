using System.Data;
using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using Binacle.Net.ServiceModule.Domain.Accounts.Models;
using Binacle.Net.ServiceModule.Domain.Accounts.Services;
using Binacle.Net.ServiceModule.Domain.Common.Models;
using Dapper;

namespace Binacle.Net.ServiceModule.Infrastructure.Accounts.Services;

internal class SqliteAccountRepository : IAccountRepository
{
	private readonly IDbConnection connection;

	public SqliteAccountRepository(IDbConnection connection)
	{
		this.connection = connection;
	}

	public async Task<FluxUnion<Account, NotFound>> GetByIdAsync(
		Guid id,
		bool allowDeleted = false,
		CancellationToken cancellationToken = default
	)
	{
		const string sql = "SELECT * FROM Accounts WHERE Id = @Id AND IsDeleted = 0 LIMIT 1";
		const string allowDeletedSql = "SELECT * FROM Accounts WHERE Id = @Id LIMIT 1";

		var dto = await this.connection.QueryFirstOrDefaultAsync<AccountDto>(
			allowDeleted ? allowDeletedSql : sql,
			new { id = id }
		);

		if (dto is null)
		{
			return TypedResult.NotFound;
		}

		return dto.ToDomain();
	}

	public async Task<FluxUnion<Account, NotFound>> GetByUsernameAsync(
		string username,
		CancellationToken cancellationToken = default)
	{
		const string sql = "SELECT * FROM Accounts WHERE Username = @Username AND IsDeleted = 0 LIMIT 1";

		var dto = await this.connection.QueryFirstOrDefaultAsync<AccountDto>(sql, new { Username = username });

		if (dto is null)
		{
			return TypedResult.NotFound;
		}

		return dto.ToDomain();
	}

	public async Task<FluxUnion<Success, Conflict>> CreateAsync(
		Account account,
		CancellationToken cancellationToken = default)
	{
		const string sql = @"
			INSERT INTO Accounts
			(Id, Username, Role, Email, Status, Password, SecurityStamp, SubscriptionId, CreatedAtUtc, IsDeleted, DeletedAtUtc)
			VALUES 
			(@Id, @Username, @Role, @Email, @Status, @Password, @SecurityStamp, @SubscriptionId, @CreatedAtUtc, @IsDeleted, @DeletedAtUtc);
		";
		try
		{
			var dto = AccountDto.FromDomain(account);
			int affectedRows = await connection.ExecuteAsync(sql, dto);
			if (affectedRows != 1)
			{
				return TypedResult.Conflict;
			}

			return TypedResult.Success;
		}
		catch (Exception ex)
		{
			return TypedResult.Conflict;
		}
	}

	public async Task<FluxUnion<Success, NotFound>> UpdateAsync(
		Account account,
		CancellationToken cancellationToken = default)
	{
		const string sql = @"
			UPDATE Accounts
			SET
				Username = @Username,
				Role = @Role,
				Email = @Email,
				Status = @Status,
				Password = @Password,
				SecurityStamp = @SecurityStamp,
				SubscriptionId = @SubscriptionId,
				CreatedAtUtc = @CreatedAtUtc,
				IsDeleted = @IsDeleted,
				DeletedAtUtc = @DeletedAtUtc
			WHERE Id = @Id
		";

		var dto = AccountDto.FromDomain(account);
		var affectedRows = await connection.ExecuteAsync(sql, dto);

		if (affectedRows <= 0)
		{
			return TypedResult.NotFound;
		}

		return TypedResult.Success;
	}

	public async Task<FluxUnion<Success, NotFound>> DeleteAsync(
		Account account,
		CancellationToken cancellationToken = default)
	{
		const string sql = @"DELETE FROM Accounts WHERE Id = @Id";
		var affectedRows = await connection.ExecuteAsync(sql, new { Id = account.Id });

		if (affectedRows <= 0)
		{
			return TypedResult.NotFound;
		}

		return TypedResult.Success;
	}

	private class AccountDto
	{
		#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
		public string Id { get; set; }
		public string Username { get; set; }
		public string Role { get; set; }
		public string Email { get; set; }
		public string Status { get; set; }
		public string? Password { get; set; }
		public string SecurityStamp { get; set; }
		public string? SubscriptionId { get; set; }
		public string CreatedAtUtc { get; set; }
		public int IsDeleted { get; set; }
		public string? DeletedAtUtc { get; set; }
		#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

		public Account ToDomain()
		{
			var password = Domain.Common.Models.Password.TryParse(this.Password);
			return new Account(
				username: this.Username,
				role: Enum.Parse<AccountRole>(this.Role),
				email: this.Email,
				status: Enum.Parse<AccountStatus>(this.Status),
				creationDate: DateTimeOffset.Parse(this.CreatedAtUtc),
				id: Guid.Parse(this.Id),
				isDeleted: this.IsDeleted == 1,
				securityStamp: Guid.Parse(this.SecurityStamp),
				password: password,
				deletionDate: this.DeletedAtUtc != null ? DateTimeOffset.Parse(this.DeletedAtUtc) : null,
				subscriptionId: this.SubscriptionId != null ? Guid.Parse(this.SubscriptionId) : null
			);
		}

		public static AccountDto FromDomain(Account entity)
		{
			return new AccountDto
			{
				Id = entity.Id.ToString(),
				Username = entity.Username,
				Role = entity.Role.ToString(),
				Email = entity.Email,
				Status = entity.Status.ToString(),
				Password = entity.Password?.ToString(),
				SecurityStamp = entity.SecurityStamp.ToString(),
				SubscriptionId = entity.SubscriptionId?.ToString(),
				CreatedAtUtc = entity.CreatedAtUtc.ToString(),
				IsDeleted = entity.IsDeleted ? 1 : 0,
				DeletedAtUtc = entity.DeletedAtUtc?.ToString()
			};
		}
	}

	internal static async Task EnsureTableExistsAsync(IDbConnection connection)
	{
		const string sql = @"
			CREATE TABLE IF NOT EXISTS Accounts (
				Id TEXT PRIMARY KEY,
				Username TEXT NOT NULL,
				Role TEXT NOT NULL,
				Email TEXT,
				Status TEXT NOT NULL,
				Password TEXT,
				SecurityStamp TEXT NOT NULL,
				SubscriptionId TEXT,
				CreatedAtUtc TEXT NOT NULL,
				IsDeleted INTEGER NOT NULL DEFAULT 0,
				DeletedAtUtc TEXT
			);
		";
		int rowsAffected = await connection.ExecuteAsync(sql);
	}
}
