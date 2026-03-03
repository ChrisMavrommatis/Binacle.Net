using Binacle.Lib.Abstractions;

namespace Binacle.Lib.AlgorithmProcessing;

public class AlgorithmProcessorFactory : IAlgorithmProcessorFactory
{
    private readonly IAlgorithmFactory algorithmFactory;

    public AlgorithmProcessorFactory(IAlgorithmFactory algorithmFactory)
    {
        this.algorithmFactory = algorithmFactory;
    }
    public IAlgorithmProcessor Create(int binCount, int itemCount)
    {
        return new LoopAlgorithmProcessor(
            [Algorithm.FFD, Algorithm.WFD, Algorithm.BFD],
            this.algorithmFactory
        );
    }
}