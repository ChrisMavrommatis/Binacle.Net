namespace Binacle.Net.ServiceModule.Infrastructure.Common.ExtensionMethods;

internal static class AsyncEnumerableExtensions
{
	public static async Task<T?> FirstOrDefaultAsync<T>(
		this IAsyncEnumerable<T> items,
		CancellationToken cancellationToken = default
	)
	{
		await using var enumerator = items.GetAsyncEnumerator(cancellationToken);
		var first = await enumerator.MoveNextAsync() ? enumerator.Current : default;
		return first;
	}
}
