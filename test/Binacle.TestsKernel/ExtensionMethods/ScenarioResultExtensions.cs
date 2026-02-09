using Binacle.Lib;
using Binacle.Lib.Abstractions.Models;
using Binacle.TestsKernel.Models;

namespace Binacle.TestsKernel.ExtensionMethods;

public static class ScenarioResultExtensions
{
	public static void EvaluateResult(this ScenarioResult expected, OperationResult actual)
	{
		var expectedStatus = actual.AlgorithmOperation switch
		{
			AlgorithmOperation.Packing => expected.PackingStatus,
			AlgorithmOperation.Fitting => expected.FittingStatus,
			_ => throw new InvalidOperationException($"Unsupported algorithm operation: {actual.AlgorithmOperation}")
		} ;
		if (expectedStatus != actual.Status)
		{
			throw new InvalidOperationException($"Operation result status mismatch. Expected: {expectedStatus}, Actual: {actual.Status}");
		}
	}
	
}
