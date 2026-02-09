using Binacle.Lib.Abstractions.Models;

namespace Binacle.TestsKernel.Helpers;

public static class ScenarioResultHelper
{
	public static Models.ScenarioResult ParseFromCompactString(string compactString)
	{
		var parts = compactString.Split(' ');
		if (parts.Length != 2)
		{
			throw new ArgumentException($"Invalid format. Value {compactString} should have format '{{Packing Operation Result Status}} {{Fitting Operation Result Status}}'.");
		}

		if (!Enum.TryParse(parts[0], out OperationResultStatus packingStatus))
		{
			throw new ArgumentException($"Invalid Packing Operation Result Status format. Value {compactString} should be a valid OperationResultStatus.");
		}
		
		if (!Enum.TryParse(parts[1], out OperationResultStatus fittingStatus))
		{
			throw new ArgumentException($"Invalid Fitting Operation Result Status format. Value {compactString} should be a valid OperationResultStatus.");
		}
		return new Models.ScenarioResult
		{
			PackingStatus = packingStatus,
			FittingStatus = fittingStatus
		};
	}
}
