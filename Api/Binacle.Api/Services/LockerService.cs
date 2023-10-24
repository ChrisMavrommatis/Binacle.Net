using Binacle.Api.Components.Services;
using Binacle.Lib.Models;

namespace Binacle.Api.Services
{
    public class LockerService : ILockerService
    {
        private readonly Binacle.Lib.StrategyFactory strategyFactory;

        public LockerService()
        {
            this.strategyFactory = new Binacle.Lib.StrategyFactory();
        }

        public Task<BinFittingOperationResult> FindFittingBinAsync(List<Item> bins, List<Item> items)
        {
            var strategy = this.strategyFactory.Create(Lib.Strategies.BinFittingStrategy.DecreasingVolumeSizeFirstFittingOrientation);

            var operation = strategy
                .WithBins(bins)
                .AndItems(items)
                .Build();

            return Task.FromResult(operation.Execute());
        }
    }
}
