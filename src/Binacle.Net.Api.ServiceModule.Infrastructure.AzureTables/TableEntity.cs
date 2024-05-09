using Azure;
using Azure.Data.Tables;

namespace Binacle.Net.Api.ServiceModule.Infrastructure.AzureTables;

internal abstract class TableEntity : ITableEntity
{
	public virtual string PartitionKey { get; set; }
	public virtual string RowKey { get; set; }
	public virtual DateTimeOffset? Timestamp { get; set; }
	public virtual ETag ETag { get; set; }
}

internal struct TableNames
{
	public const string Users = "users";
}
