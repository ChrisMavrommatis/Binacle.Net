using Binacle.Net.Lib.Models;

namespace Binacle.Net.Api.Services
{
    public class LockerService : ILockerService
    {
        private readonly Lib.StrategyFactory strategyFactory;

        public LockerService()
        {
            this.strategyFactory = new Lib.StrategyFactory();
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
