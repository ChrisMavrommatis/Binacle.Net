using Binacle.Lib.Abstractions.Strategies;
using Binacle.Lib.Strategies;

namespace Binacle.Lib
{
    public class StrategyFactory
    {
        public IBinFittingStrategy Create(BinFittingStrategy strategyType)
        {
            switch (strategyType)
            {
                case BinFittingStrategy.DecreasingVolumeSizeFirstFittingOrientation:
                    return new Strategies.DecreasingVolumeSize_v1();
                default:
                    throw new NotImplementedException($"No Bin Fitting Strategy exists for {strategyType}");
            }
        }
    }
}
