using Binacle.Net.Lib;
using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Api.DiagnosticsModule.ExtensionMethods;

internal static class LogProcessorHandlingExtensions
{
	public static Dictionary<string, object> ConvertToLogFormat(
		this IEnumerable<IWithReadOnlyDimensions> items
		)
	{
		return items.ToDictionary(
			(x) => ((IWithID)x).ID, 
			x => (object)x.FormatDimensions()
		);
	}
	
	public static string FormatDimensionsAndCoordinates(this Lib.Packing.Models.ResultItem item)
	{
		if(item.Coordinates is not null)
			return $"{item.Dimensions.FormatDimensions()} {item.Coordinates.Value.FormatCoordinates()}";

		return $"{item.Dimensions.FormatDimensions()}";
	}
	
	// public static IDisposable? EnrichStateWithParameters(this ILogger logger, Models.LegacyFittingParameters parameters)
	// {
	// 	List<string> parametersList = [];
	//
	// 	if (parameters.ReportFittedItems)
	// 	{
	// 		parametersList.Add("ReportFittedItems");
	// 	}
	//
	// 	if (parameters.ReportUnfittedItems)
	// 	{
	// 		parametersList.Add("ReportUnfittedItems");
	// 	}
	//
	// 	if (parameters.FindSmallestBinOnly)
	// 	{
	// 		parametersList.Add("FindSmallestBinOnly");
	// 	}
	//
	// 	return logger.EnrichState("Parameters", parametersList);
	// }
	//
	// public static IDisposable? EnrichStateWithParameters(this ILogger logger, Models.LegacyPackingParameters parameters)
	// {
	// 	List<string> parametersList = new List<string>();
	//
	// 	if (parameters.OptInToEarlyFails)
	// 	{
	// 		parametersList.Add("OptInToEarlyFails");
	// 	}
	//
	// 	if (parameters.ReportPackedItemsOnlyWhenFullyPacked)
	// 	{
	// 		parametersList.Add("ReportPackedItemsOnlyWhenFullyPacked");
	// 	}
	//
	// 	if (parameters.NeverReportUnpackedItems)
	// 	{
	// 		parametersList.Add("NeverReportUnpackedItems");
	// 	}
	//
	// 	if (parameters.StopAtSmallestBin)
	// 	{
	// 		parametersList.Add("StopAtSmallestBin");
	// 	}
	//
	// 	return logger.EnrichState("Parameters", parametersList);
	// }
	//
	// public static IDisposable? EnrichStateWithParameters(this ILogger logger, Models.PackingParameters parameters)
	// {
	// 	List<string> parametersList =
	// 	[
	// 		parameters.Algorithm.ToString()
	// 	];
	//
	// 	return logger.EnrichState("Parameters", parametersList);
	// }

	
	public static Dictionary<string, object> ConvertToLogFormat(
		this Dictionary<string, Lib.Packing.Models.PackingResult> results
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
						group => group.Select(item => item.FormatDimensionsAndCoordinates()).ToArray()
					);
				resultState.Add("PackedItems", packedItems);
			}
			if (value.UnpackedItems is not null)
			{
				var unpackedItems = value.UnpackedItems!
					.GroupBy(x => x.ID)
					.ToDictionary(
						group => group.Key,
						group => group.Select(item => item.FormatDimensionsAndCoordinates()).ToArray()
					);
				resultState.Add("UnpackedItems", unpackedItems);
			}
			state.Add(key, resultState);
		}

		return state;
	}
	
	public static Dictionary<string, object> ConvertToLogFormat(
		this Dictionary<string, Lib.Fitting.Models.FittingResult> results
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
