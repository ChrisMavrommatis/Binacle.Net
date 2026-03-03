using Binacle.Lib.Abstractions;

namespace Binacle.Lib;

public class BinProcessorFactory : IBinProcessorFactory
{
    private readonly IAlgorithmFactory algorithmFactory;

    public BinProcessorFactory(IAlgorithmFactory algorithmFactory)
    {
        this.algorithmFactory = algorithmFactory;
    }
    public IBinProcessor Create(int binCount, int itemCount)
    {
        return new LoopBinProcessor(
            this.algorithmFactory
        );
    }
}