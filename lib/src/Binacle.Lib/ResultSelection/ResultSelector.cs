using Binacle.Lib.Abstractions;
using Binacle.Lib.Abstractions.Models;

namespace Binacle.Lib.ResultSelection;

public class ResultSelector : IResultSelector
{
    private readonly IResultSelectionStrategy bestBin;
    private readonly IResultSelectionStrategy smallestBin;
    private readonly IResultSelectionStrategy bestAlgorithm;

    public ResultSelector(
        IResultSelectionStrategy bestBin,
        IResultSelectionStrategy smallestBin,
        IResultSelectionStrategy bestAlgorithm
        )
    {
        this.bestBin = bestBin;
        this.smallestBin = smallestBin;
        this.bestAlgorithm = bestAlgorithm;
    }
    
    
    public OperationResult BestBin(IDictionary<string, OperationResult> results)
    {
        return this.bestBin.Select(results);
    }

    public OperationResult SmallestBin(IDictionary<string, OperationResult> results)
    {
        return this.smallestBin.Select(results);
    }

    public OperationResult BestAlgorithm(IDictionary<string, OperationResult> results)
    {
        return this.bestAlgorithm.Select(results);
    }
}
