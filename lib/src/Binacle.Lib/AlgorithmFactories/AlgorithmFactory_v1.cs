using Binacle.Lib.Abstractions;
using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Models;

namespace Binacle.Lib;

internal class AlgorithmFactory_v1 : IAlgorithmFactory
{
    public IPackingAlgorithm Create<TBin, TItem>(Algorithm algorithm, TBin bin, IList<TItem> items)
        where TBin : class, IWithID, IWithReadOnlyDimensions
        where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
    {
        var algorithmInstance = (IPackingAlgorithm)(algorithm switch
        {
            Algorithm.FFD => new Algorithms.FirstFitDecreasing_v1<TBin, TItem>(bin, items),
            Algorithm.WFD => new Algorithms.WorstFitDecreasing_v1<TBin, TItem>(bin, items),
            Algorithm.BFD => new Algorithms.BestFitDecreasing_v1<TBin, TItem>(bin, items),
            _ => throw new NotSupportedException($"No Packing Algorithm exists for {algorithm}")
        });

        return algorithmInstance;
    }
}