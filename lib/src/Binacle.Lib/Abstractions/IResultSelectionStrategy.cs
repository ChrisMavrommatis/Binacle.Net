namespace Binacle.Lib.Abstractions;

public interface IResultSelectionStrategy
{
    public OperationResult Select(IDictionary<string, OperationResult> results);
}

