using Binacle.Net.Lib.Abstractions.Models;
using ChrisMavrommatis.Logging;
using Newtonsoft.Json.Linq;

namespace Binacle.Net.Api.ExtensionMethods;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public static class LoggerExtensions
{
	public static IDisposable? EnrichStateWith<TItem>(this ILogger logger, string name, IEnumerable<TItem> value)
		where TItem : IWithID, IWithReadOnlyDimensions
	{

		Dictionary<string, object> state = value.ToItemDimensionDictionary();
		return logger.EnrichState(name, state);
	}

	public static IDisposable? EnritchStateWithResults(this ILogger logger, Dictionary<string, Lib.Fitting.Models.FittingResult> results)
	{
		Dictionary<string, object> state = new Dictionary<string, object>();

		foreach (var (key, value) in results)
		{
			Dictionary<string, object> resultState = new Dictionary<string, object>
			{
				{ "Status", value.Status },
			};
			if (value.Reason.HasValue)
			{
				resultState.Add("Reason", value.Reason.Value);
			}
			if (value.FittedItems is not null)
			{
				resultState.Add("FittedItems", value.FittedItems!.ToItemDimensionDictionary());
			}
			if (value.UnfittedItems is not null)
			{
				resultState.Add("UnfittedItems", value.UnfittedItems!.ToItemDimensionDictionary());
			}
			if (value.FittedBinVolumePercentage.HasValue)
			{
				resultState.Add("FittedBinVolumePercentage", value.FittedBinVolumePercentage.Value);
			}
			if (value.FittedItemsVolumePercentage.HasValue)
			{
				resultState.Add("FittedItemsVolumePercentage", value.FittedItemsVolumePercentage.Value);
			}
			state.Add(key, resultState);
		}

		return logger.EnrichState("Results", state);
	}
	public static IDisposable? EnritchStateWithResults(this ILogger logger, Lib.Packing.Models.PackingResult value)
	{
		Dictionary<string, object> state = new Dictionary<string, object>
		{
			{ "Status", value.Status },
		};

		if (value.PackedItems is not null)
		{
			state.Add("PackedItems", value.PackedItems!.ToItemDimensionDictionary());
		}
		return logger.EnrichState("Results", state);
	}
}
