using Binacle.Lib.Abstractions;
using Binacle.Lib.AlgorithmProcessing;

namespace Binacle.Lib;

public class BinProcessorFactory : IBinProcessorFactory
{
    private readonly IAlgorithmFactory algorithmFactory;
    private readonly IResultSelector resultSelector;

    public BinProcessorFactory(
	    IAlgorithmFactory algorithmFactory,
	    IResultSelector resultSelector
	    )
    {
	    this.algorithmFactory = algorithmFactory;
	    this.resultSelector = resultSelector;
    }
    public IBinProcessor Create(int binCount, int itemCount)
    {
        return new LoopBinProcessor(
            this.algorithmFactory
        );
    }
    
    public IMultiAlgorithmBinProcessor CreateMultiAlgorithm(int binCount, int itemCount)
	{
		var algorithmProcessor = new LoopAlgorithmProcessor(
			[Algorithm.FFD, Algorithm.BFD],
			this.algorithmFactory
		);

		return new LoopMultiAlgorithmBinProcessor(
			algorithmProcessor,
			this.resultSelector
		);
	}
}
