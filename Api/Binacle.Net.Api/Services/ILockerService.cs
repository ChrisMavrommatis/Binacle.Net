using Binacle.Net.Lib.Models;

namespace Binacle.Net.Api.Services
{
    public interface ILockerService
    {
        public Task<BinFittingOperationResult> FindFittingBinAsync(List<Item> bins, List<Item> items);
    }
}
