using Binacle.Lib.Abstractions;
using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Models;

namespace Binacle.Lib.AlgorithmProcessing;

public class LoopAlgorithmProcessor: IAlgorithmProcessor
{
    private readonly Algorithm[] supportedAlgorithms;
    private readonly IAlgorithmFactory algorithmFactory;

    public LoopAlgorithmProcessor(
        Algorithm[] supportedAlgorithms,
        IAlgorithmFactory algorithmFactory
        )
    {
        this.supportedAlgorithms = supportedAlgorithms;
        this.algorithmFactory = algorithmFactory;
    }
    
    public IDictionary<string, OperationResult> Process<TBin, TItem>(
        TBin bin, 
        IList<TItem> items, 
        IOperationParameters parameters,
        CancellationToken cancellationToken = default
        ) 
        where TBin : class, IWithID, IWithReadOnlyDimensions 
        where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
    {
        using var activity = Diagnostics.ActivitySource
            .StartActivity($"Process Algorithms: Loop");
        activity?.SetTag("Operation", parameters.Operation);
        var results = new Dictionary<string, OperationResult>(this.supportedAlgorithms.Length);

        for (var i = 0; i < this.supportedAlgorithms.Length; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var algorithm = this.supportedAlgorithms[i];
            var algorithmInstance = this.algorithmFactory.Create(algorithm, bin, items);
            var result = algorithmInstance.Execute(parameters);
            results[algorithmInstance.GetAlgorithmIdentifierName()] = result;
        }

        return results;
    }
}
