using Binacle.Lib.Abstractions;
using Binacle.Lib.Abstractions.Models;

namespace Binacle.TestsKernel.Models;

public class ResultSelectionScenario
{
    public string ExpectedResult { get; init; }
    public Dictionary<string, OperationResult> Results { get; init; }
    public IResultSelectionStrategy[] Strategies  { get; init; }
}

