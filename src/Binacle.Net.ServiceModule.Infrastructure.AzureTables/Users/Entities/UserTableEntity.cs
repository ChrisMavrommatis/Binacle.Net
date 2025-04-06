using Azure;
using Azure.Data.Tables;
using Binacle.Net.ServiceModule.Domain.Users.Entities;

namespace Binacle.Net.ServiceModule.Infrastructure.AzureTables.Users.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

internal class UserTableEntity : User, ITableEntity
{
	public UserTableEntity(string rowKey, string partitionKey)
	{
		this.RowKey = rowKey;
		this.PartitionKey = partitionKey;
	}

	public UserTableEntity()
	{

	}

	public string PartitionKey { get; set; }
	public string RowKey { get; set; }
	public DateTimeOffset? Timestamp { get; set; }
	public ETag ETag { get; set; }
}

internal static class UserTableEntityExtensions
{
	internal static User ToDomainUser(this UserTableEntity tableEntity)
	{
		return new User
		{
			Email = tableEntity.Email,
			NormalizedEmail = tableEntity.NormalizedEmail,
			Group = tableEntity.Group,
			Salt = tableEntity.Salt,
			HashedPassword = tableEntity.HashedPassword,
			CreatedAtUtc = tableEntity.CreatedAtUtc,
			IsActive = tableEntity.IsActive,
			IsDeleted = tableEntity.IsDeleted,
			DeletedAtUtc = tableEntity.DeletedAtUtc,
		};
	}

	internal static UserTableEntity ToTableEntityUser(this User entity, string partitionKey)
	{
		return new UserTableEntity(entity.NormalizedEmail, partitionKey)
		{
			Email = entity.Email,
			NormalizedEmail = entity.NormalizedEmail,
			Group = entity.Group,
			Salt = entity.Salt,
			HashedPassword = entity.HashedPassword,
			CreatedAtUtc = entity.CreatedAtUtc,
			IsActive = entity.IsActive,
			IsDeleted = entity.IsDeleted,
			DeletedAtUtc = entity.DeletedAtUtc,
		};
	}
}
