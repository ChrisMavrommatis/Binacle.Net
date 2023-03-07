using Binacle.Lib.Components.Strategies;

namespace Binacle.Lib
{
    public class StrategyFactory
    {
        public IBinFittingStrategy Create(BinFittingStrategy strategyType)
        {
            switch (strategyType)
            {
                case BinFittingStrategy.DecreasingVolumeSizeFirstFittingOrientation:
                    return new Strategies.DecreasingVolumeSizeFirstFittingOrientation();
                default:
                    throw new NotImplementedException($"No Bin Fitting Strategy exists for {strategyType}");
            }
        }
    }
}
