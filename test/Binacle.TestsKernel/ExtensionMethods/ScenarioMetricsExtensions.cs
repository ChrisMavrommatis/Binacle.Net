using Binacle.Lib.Abstractions.Models;
using Binacle.TestsKernel.Models;

namespace Binacle.TestsKernel.ExtensionMethods;

public static class ScenarioMetricsExtensions
{
	public static void EvaluateResult(this ScenarioMetrics metrics, OperationResult result)
	{
		var totalItemsVolume = result.TotalItemsVolume();
		if (metrics.ItemsVolume != totalItemsVolume)
		{
			throw new InvalidOperationException($"Items volume mismatch. Expected: {metrics.ItemsVolume}, Actual: {totalItemsVolume}");
		}
		if (metrics.BinVolume != result.Bin.Volume)
		{
			throw new InvalidOperationException($"Bin volume mismatch. Expected: {metrics.BinVolume}, Actual: {result.Bin.Volume}");
		}

		var totalItemsCount = result.TotalItemsCount();
		if (metrics.ItemsCount != totalItemsCount)
		{
			throw new InvalidOperationException($"Items count mismatch. Expected: {metrics.ItemsCount}, Actual: {totalItemsCount}");
		}

		if (metrics.Percentage <= result.PackedBinVolumePercentage)
		{
			throw new InvalidOperationException($"Packed volume percentage too high. Expected at most: {metrics.Percentage}, Actual: {result.PackedBinVolumePercentage}");
		}
	}
	
}
