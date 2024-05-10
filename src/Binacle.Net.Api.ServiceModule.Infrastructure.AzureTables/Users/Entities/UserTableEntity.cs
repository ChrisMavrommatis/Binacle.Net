namespace Binacle.Net.Api.ServiceModule.Infrastructure.AzureTables.Users.Entities;

internal class UserTableEntity : TableEntity
{
	public UserTableEntity(string rowKey, string partitionKey)
	{
		this.RowKey = rowKey;
		this.PartitionKey = partitionKey;
	}
	public UserTableEntity()
	{

	}
	public string Email { get => RowKey; }
	public string Group { get; set; }
	public string HashedPassword { get; set; }
	public string Salt { get; set; }
	public bool IsActive { get; set; }
}
