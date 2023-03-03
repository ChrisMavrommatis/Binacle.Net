using Binacle.Lib.Components.Models;

namespace Binacle.Lib.Components.Strategies
{
    public interface IBinFittingStrategy
    {
        IBinFittingStrategyWithBins WithBins(List<Bin> bins);
    }
}
