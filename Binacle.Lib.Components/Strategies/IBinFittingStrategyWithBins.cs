namespace Binacle.Lib.Components.Strategies
{
    public interface IBinFittingStrategyWithBins
    {
        IBinFittingStrategyWithBinsAndItems AndItems(IEnumerable<Binacle.Lib.Components.Models.Item> items);
    }
}
