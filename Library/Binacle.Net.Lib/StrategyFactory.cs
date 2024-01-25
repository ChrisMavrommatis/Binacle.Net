using Binacle.Net.Lib.Abstractions.Strategies;
using Binacle.Net.Lib.Strategies;

namespace Binacle.Net.Lib
{
    public class StrategyFactory
    {
        public IBinFittingStrategy Create(BinFittingStrategy strategyType)
        {
            
            var strategy = (IBinFittingStrategy)(strategyType switch
            {
                BinFittingStrategy.DecreasingVolumeSize => new Strategies.DecreasingVolumeSize_v1(),
                _ => throw new NotImplementedException($"No Bin Fitting Strategy exists for {strategyType}")
            });

            return strategy;
        }
    }
}
