using Binacle.Net.Kernel.Logs.Models;
using Binacle.Lib;
using Binacle.Lib.Abstractions.Models;
using Binacle.CompactNotation;

namespace Binacle.Net.DiagnosticsModule.ExtensionMethods;

internal static class LogProcessorHandlingExtensions
{
	public static Dictionary<string, object> ConvertToLogObject(
		this AlgorithmOperationLogChannelRequest request)
	{
		var log = new Dictionary<string, object>();
		log.Add("Bins", request.Bins.ConvertToLogObject());
		log.Add("Items", request.Items.ConvertToLogObject());
		if (request.Parameters is not null)
		{
			log.Add("Parameters", request.Parameters.ConvertToLogObject());
		}

		log.Add("Results", request.Results.ConvertToLogObject());
		return log;
	}

	private static Dictionary<string, object> ConvertToLogObject(
		this IEnumerable<IWithReadOnlyDimensions> items
	)
	{
		return items.ToDictionary(
			(x) => ((IWithID)x).ID,
			x => (object)CompactNotationFormatter.Format<int>(x)
		);
	}

	private static string ConvertToLogObject(this PackedItem item)
	{
		return CompactNotationFormatter.Format<int>(item);
	}

	private static string ConvertToLogObject(this UnpackedItem item)
	{
		return CompactNotationFormatter.Format<int>(item);
	}

	private static Dictionary<string, object> ConvertToLogObject(
		this IDictionary<string, OperationResult> results
	)
	{
		Dictionary<string, object> state = new Dictionary<string, object>();

		foreach (var (key, value) in results)
		{
			Dictionary<string, object> resultState = new Dictionary<string, object>
			{
				{ "Status", value.Status.ToString() },
				{ "PackedBinVolumePercentage", value.PackedBinVolumePercentage },
				{ "PackedItemsVolumePercentage", value.PackedItemsVolumePercentage }
			};

			var packedItems = value.PackedItems!
				.GroupBy(x => x.ID)
				.ToDictionary(
					group => group.Key,
					group => group.Select(item => 
						item.ConvertToLogObject()
					).ToArray()
				);
			resultState.Add("PackedItems", packedItems);

			var unpackedItems = value.UnpackedItems!
				.GroupBy(x => x.ID)
				.ToDictionary(
					group => group.Key,
					group => group.Select(item => item.ConvertToLogObject()).ToArray()
				);
			resultState.Add("UnpackedItems", unpackedItems);

			state.Add(key, resultState);
		}

		return state;
	}
}
