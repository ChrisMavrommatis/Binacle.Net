
namespace Binacle.TestsKernel.Algorithms.Models;

public class ScenarioResult
{
	public OperationResultStatus PackingStatus { get; init; }
	public EarlyExitReason PackingEarlyExitReason { get; init; }
	public OperationResultStatus FittingStatus { get; init; }
	public EarlyExitReason FittingEarlyExitReason { get; init; }
	
}
