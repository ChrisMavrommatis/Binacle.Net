namespace Binacle.Lib.Abstractions;

public interface IResultSelector
{
    public OperationResult BestBin(IDictionary<string, OperationResult> results);
    public OperationResult SmallestBin(IDictionary<string, OperationResult> results);
    public OperationResult BestAlgorithm(IDictionary<string, OperationResult> results);
}
