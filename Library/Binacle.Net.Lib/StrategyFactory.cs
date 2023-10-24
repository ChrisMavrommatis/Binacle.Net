﻿using Binacle.Net.Lib.Abstractions.Strategies;
using Binacle.Net.Lib.Strategies;

namespace Binacle.Net.Lib
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