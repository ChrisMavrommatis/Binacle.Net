using System.Collections.Concurrent;
using Binacle.Lib.Abstractions;
using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Models;

namespace Binacle.Lib.AlgorithmProcessing;

public class ParallelAlgorithmProcessor: IAlgorithmProcessor
{
    private readonly Algorithm[] supportedAlgorithms;
    private readonly IAlgorithmFactory algorithmFactory;
    private readonly int concurrencyLevel;

    public ParallelAlgorithmProcessor(
        Algorithm[] supportedAlgorithms,
        IAlgorithmFactory algorithmFactory,
        int? concurrencyLevel = null
    )
    {
        this.supportedAlgorithms = supportedAlgorithms;
        this.algorithmFactory = algorithmFactory;
        this.concurrencyLevel = concurrencyLevel ?? Environment.ProcessorCount;
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
            .StartActivity($"Process Algorithms: Parallel");
        activity?.SetTag("Operation", parameters.Operation);
        var results = new ConcurrentDictionary<string, OperationResult>(this.concurrencyLevel, this.supportedAlgorithms.Length);

        var parallelOptions = new ParallelOptions { CancellationToken = cancellationToken };
        Parallel.For(0, this.supportedAlgorithms.Length, parallelOptions, i =>
        {
            var algorithm = this.supportedAlgorithms[i];
            var algorithmInstance = this.algorithmFactory.Create(algorithm, bin, items);
            var result = algorithmInstance.Execute(parameters);
            results[algorithmInstance.GetAlgorithmIdentifierName()] = result;
        });
        return results;
    }
}