using Binacle.Lib.Abstractions.Models;

namespace Binacle.TestsKernel.Algorithms.Helpers;

public static class ScenarioResultHelper
{
	public static Models.ScenarioResult ParseFromCompactString(string compactString)
	{
		var parts = compactString.Split(' ');
		if (parts.Length != 2)
		{
			throw new ArgumentException(
				$"Invalid format. Value {compactString} should have format '{{Packing Operation Result Status}} {{Fitting Operation Result Status}}'.");
		}

		var (packingStatus, packingEarlyExitReason) = ParseOperationResultStatus(parts[0]);
		var (fittingStatus, fittingEarlyExitReason) = ParseOperationResultStatus(parts[1]);
		return new Models.ScenarioResult
		{
			PackingStatus = packingStatus,
			PackingEarlyExitReason = packingEarlyExitReason,
			FittingStatus = fittingStatus, 
			FittingEarlyExitReason = fittingEarlyExitReason
		};
	}

	private static (OperationResultStatus resultStatus, EarlyExitReason earlyExitReason) ParseOperationResultStatus(
		string compactString)
	{
		if (!compactString.Contains("-"))
		{
			if (!Enum.TryParse(compactString, out OperationResultStatus parsedStatus))
			{
				throw new ArgumentException(
					$"Invalid Operation Result Status format. Value {compactString} should be a valid OperationResultStatus.");
			}

			return (parsedStatus, EarlyExitReason.None);
		}

		var parts = compactString.Split('-');
		if (parts.Length != 2)
		{
			throw new ArgumentException(
				$"Invalid format. Value {compactString} should have format '{{Operation Result Status}}-{{Early Exit Reason}}'.");
		}

		if (!Enum.TryParse(parts[0], out OperationResultStatus status))
		{
			throw new ArgumentException(
				$"Invalid Operation Result Status format. Value {compactString} should be a valid OperationResultStatus.");
		}

		if (!Enum.TryParse(parts[1], out EarlyExitReason earlyExitReason))
		{
			throw new ArgumentException(
				$"Invalid Early Exit Reason format. Value {compactString} should be a valid EarlyExitReason.");
		}

		return (status, earlyExitReason);
	}
}
