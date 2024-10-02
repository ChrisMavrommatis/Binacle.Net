using Azure.Data.Tables;

namespace Binacle.Net.Api.ServiceModule.Infrastructure.AzureTables.ExtensionMethods;

internal static class TableClientExtensions
{
	public static async Task<T?> QuerySingleAsync<T>(this TableClient client, System.Linq.Expressions.Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default)
		where T : class, ITableEntity
	{
		var pages = client.QueryAsync<T>(filter, 1, cancellationToken: cancellationToken);
		await foreach(var page in pages)
		{
			return page;
		}
		return null;
	}
}
