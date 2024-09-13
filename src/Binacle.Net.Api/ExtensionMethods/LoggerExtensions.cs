using Binacle.Net.Lib.Abstractions.Models;
using ChrisMavrommatis.Logging;

namespace Binacle.Net.Api.ExtensionMethods;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public static class LoggerExtensions
{
	public static IDisposable? EnrichStateWith<TItem>(this ILogger logger, string name, IEnumerable<TItem> value)
		where TItem : IWithID, IWithReadOnlyDimensions
	{

		var state = value.ToDictionary(x => x.ID, x => $"{x.Height}x{x.Length}x{x.Width}");
		return logger.EnrichState(
			name, 
			state as  IEnumerable<KeyValuePair<string, object>>
		);
	}

	public static IDisposable? EnrichStateWh<TItem>(this ILogger logger, string name, IEnumerable<TItem> value)
		where TItem : IWithID, IWithReadOnlyDimensions, IWithQuantity
	{

		var state = value.ToDictionary(x => x.ID, x => $"{x.Height}x{x.Length}x{x.Width}");
		return logger.EnrichState(
			name,
			state as IEnumerable<KeyValuePair<string, object>>
		);
	}
}
