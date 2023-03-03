using Binacle.Api.Components.Services;
using Binacle.Lib.Components.Models;

namespace Binacle.Api.Services
{
    public class LockerService : ILockerService
    {
        private readonly Binacle.Lib.StrategyFactory strategyFactory;

        public LockerService()
        {
            this.strategyFactory = new Binacle.Lib.StrategyFactory();
        }

        public BinFittingOperationResult FindFittingBin(List<Bin> bins, List<Item> items)
        {
            var strategy = this.strategyFactory.Create(Lib.Components.Strategies.BinFittingStrategy.DecreasingVolumeSizeFirstFittingOrientation);

            var operation = strategy
                .WithBins(bins)
                .AndItems(items)
                .Build();

            return operation.Execute();
        }
    }
}
