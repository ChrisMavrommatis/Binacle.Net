namespace Binacle.Net.Api.ServiceModule.Infrastructure.AzureTables.Users.Entities;

internal class UserTableEntity : TableEntity
{
	public UserTableEntity(string email, string group)
	{
		this.RowKey = email;
		this.PartitionKey = group;
		this.Group = group;
	}
	public UserTableEntity()
	{

	}
	public string Email { get => RowKey; }
	public string Group { get; set; }
	public string HashedPassword { get; set; }
	public string Salt { get; set; }
}
