using Binacle.Lib.Components.Models;

namespace Binacle.Lib.Components.Strategies
{
    public interface IBinFittingStrategyWithBins
    {
        IBinFittingStrategyWithBinsAndItems AndItems(List<Item> items);
    }
}
