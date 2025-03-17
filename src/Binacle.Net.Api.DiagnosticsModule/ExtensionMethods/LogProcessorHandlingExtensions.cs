using Binacle.Net.Api.Kernel.Models;
using Binacle.Net.Lib;
using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Api.DiagnosticsModule.ExtensionMethods;

internal static class LogProcessorHandlingExtensions
{
	public static Dictionary<string, object> ConvertToLogObject(
		this PackingLogChannelRequestBase request)
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
	
	public static Dictionary<string, object> ConvertToLogObject(
		this FittingLogChannelRequestBase request)
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
			x => (object)x.FormatDimensions()
		);
	}

	private static string ConvertToLogObject(this Lib.Packing.Models.ResultItem item)
	{
		if (item.Coordinates is not null)
			return $"{item.Dimensions.FormatDimensions()} {item.Coordinates.Value.FormatCoordinates()}";

		return $"{item.Dimensions.FormatDimensions()}";
	}

	private static Dictionary<string, object> ConvertToLogObject(
		this IDictionary<string, Lib.Packing.Models.PackingResult> results
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

			if (value.PackedItems is not null)
			{
				var packedItems = value.PackedItems!
					.GroupBy(x => x.ID)
					.ToDictionary(
						group => group.Key,
						group => group.Select(item => item.ConvertToLogObject()).ToArray()
					);
				resultState.Add("PackedItems", packedItems);
			}

			if (value.UnpackedItems is not null)
			{
				var unpackedItems = value.UnpackedItems!
					.GroupBy(x => x.ID)
					.ToDictionary(
						group => group.Key,
						group => group.Select(item => item.ConvertToLogObject()).ToArray()
					);
				resultState.Add("UnpackedItems", unpackedItems);
			}

			state.Add(key, resultState);
		}

		return state;
	}

	private static Dictionary<string, object> ConvertToLogObject(
		this IDictionary<string, Lib.Fitting.Models.FittingResult> results
	)
	{
		Dictionary<string, object> state = new Dictionary<string, object>();
		foreach (var (key, value) in results)
		{
			Dictionary<string, object> resultState = new Dictionary<string, object>
			{
				{ "Status", value.Status.ToString() },
			};
			if (value.Reason.HasValue)
			{
				resultState.Add("Reason", value.Reason.Value);
			}

			if (value.FittedItems is not null)
			{
				var fittedItems = value.FittedItems!
					.GroupBy(x => x.ID)
					.ToDictionary(
						group => group.Key,
						group => group.Select(item => item.FormatDimensions()).ToArray()
					);
				resultState.Add("FittedItems", fittedItems);
			}

			if (value.UnfittedItems is not null)
			{
				var unfittedItems = value.UnfittedItems!
					.GroupBy(x => x.ID)
					.ToDictionary(
						group => group.Key,
						group => group.Select(item => item.FormatDimensions()).ToArray()
					);
				resultState.Add("UnfittedItems", unfittedItems);
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

		return state;
	}
}
