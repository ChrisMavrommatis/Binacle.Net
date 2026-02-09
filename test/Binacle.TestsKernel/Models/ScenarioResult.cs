using Binacle.Lib.Abstractions.Models;

namespace Binacle.TestsKernel.Models;

public class ScenarioResult
{
	public OperationResultStatus PackingStatus { get; init; }
	public OperationResultStatus FittingStatus { get; init; }
}
