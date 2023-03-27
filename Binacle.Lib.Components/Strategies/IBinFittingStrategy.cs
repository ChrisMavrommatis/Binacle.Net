namespace Binacle.Lib.Components.Strategies
{
    public interface IBinFittingStrategy
    {
        IBinFittingStrategyWithBins WithBins(IEnumerable<Binacle.Lib.Components.Models.Item> bins);
    }
}
