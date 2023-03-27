using Binacle.Lib.Components.Models;

namespace Binacle.Api.Components.Services
{
    public interface ILockerService
    {
        public Task<BinFittingOperationResult> FindFittingBinAsync(List<Item> bins, List<Item> items);
    }
}
