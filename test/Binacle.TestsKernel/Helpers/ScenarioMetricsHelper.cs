namespace Binacle.TestsKernel.Helpers;

public static class ScenarioMetricsHelper
{
	public static Models.ScenarioMetrics ParseFromCompactString(string compactString)
	{
		var parts = compactString.Split(' ');
		if (parts.Length != 4)
		{
			throw new ArgumentException($"Invalid format. Value {compactString} should have format 'ItemsVolume BinVolume ItemsCount Percentage'.");
		}
		if (!int.TryParse(parts[0], out int itemsVolume)
		    || !int.TryParse(parts[1], out int binVolume)
		    || !int.TryParse(parts[2], out int itemsCount)
		    || !decimal.TryParse(parts[3].TrimEnd('%'), out decimal percentage)
		   )
		{
			throw new ArgumentException($"Invalid number format. Value {compactString} should have format 'ItemsVolume BinVolume ItemsCount Percentage'.");
		}

		return new Models.ScenarioMetrics
		{
			ItemsVolume = itemsVolume,
			BinVolume = binVolume,
			ItemsCount = itemsCount,
			Percentage = percentage
		};
	}
}
