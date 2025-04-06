using Azure;
using Azure.Data.Tables;

namespace Binacle.Net.ServiceModule.Infrastructure.AzureTables;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

internal abstract class TableEntity : ITableEntity
{
	public virtual string PartitionKey { get; set; }
	public virtual string RowKey { get; set; }
	public virtual DateTimeOffset? Timestamp { get; set; }
	public virtual ETag ETag { get; set; }
}
